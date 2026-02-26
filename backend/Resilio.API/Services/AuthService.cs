using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Resilio.Core.DTOs;
using Resilio.Core.Interfaces;
using Resilio.Core.Models;

namespace Resilio.API.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IOtpRepository _otps;
    private readonly IAuditLogRepository _audit;
    private readonly IJwtTokenService _jwt;
    private readonly IOtpHasher _hasher;
    private readonly IOtpGenerator _generator;
    private readonly int _otpExpiryMinutes;

    public AuthService(
        IUserRepository users,
        IOtpRepository otps,
        IAuditLogRepository audit,
        IJwtTokenService jwt,
        IOtpHasher hasher,
        IOtpGenerator generator,
        IConfiguration config)
    {
        _users = users;
        _otps = otps;
        _audit = audit;
        _jwt = jwt;
        _hasher = hasher;
        _generator = generator;
        _otpExpiryMinutes = int.TryParse(config["Otp:ExpiryMinutes"], out var m) ? m : 5;
    }

    public async Task<AuthStartResponse> StartAsync(AuthStartRequest request, string? ip, string? userAgent, CancellationToken ct)
    {
        var identifier = request.Identifier.Trim();
        var channel = request.Channel.Trim().ToUpperInvariant();
        var role = request.Role.Trim();

        // Basic validation (keep it simple)
        if (string.IsNullOrWhiteSpace(identifier)) throw new ArgumentException("Identifier is required.");
        if (channel is not ("SMS" or "EMAIL")) throw new ArgumentException("Channel must be SMS or EMAIL.");
        if (role is not ("Victim" or "Volunteer")) throw new ArgumentException("Role must be Victim or Volunteer.");

        // Generate OTP and hash it
        var otp = _generator.Generate6Digits();
        var hash = _hasher.Hash(identifier, otp);
        var expiresAt = DateTime.UtcNow.AddMinutes(_otpExpiryMinutes);

        // Save OTP request
        await _otps.InsertAsync(identifier, channel, hash, expiresAt, ct);

        // Audit log (do NOT store otp in logs)
        await _audit.WriteAsync(
            userId: null,
            action: "OTP_SENT",
            metadataJson: JsonSerializer.Serialize(new { identifier, channel, role, expiresAt }),
            ip: ip,
            userAgent: userAgent,
            ct: ct);

        // IMPORTANT:
        // In real system, you'd send otp via SMS/Email provider.
        // For now: return generic message only (anti-enumeration)
        // For development you can temporarily print otp in server console if needed.
        Console.WriteLine($"[DEV ONLY] OTP for {identifier}: {otp}");

        return new AuthStartResponse("If this account exists, we sent a verification code.");
    }

    public async Task<AuthVerifyResponse> VerifyAsync(AuthVerifyRequest request, string? ip, string? userAgent, CancellationToken ct)
    {
        var identifier = request.Identifier.Trim();
        var otp = request.Otp.Trim();
        var role = request.Role.Trim();

        if (string.IsNullOrWhiteSpace(identifier)) throw new ArgumentException("Identifier is required.");
        if (otp.Length != 6) throw new ArgumentException("OTP must be 6 digits.");
        if (role is not ("Victim" or "Volunteer")) throw new ArgumentException("Role must be Victim or Volunteer.");

        var now = DateTime.UtcNow;
        var record = await _otps.GetLatestValidAsync(identifier, now, ct);

        if (record is null)
        {
            await _audit.WriteAsync(null, "OTP_VERIFY_FAILED", JsonSerializer.Serialize(new { identifier, reason = "NO_VALID_OTP" }), ip, userAgent, ct);
            throw new UnauthorizedAccessException("Invalid or expired OTP.");
        }

        // Prevent brute force (simple attempt check)
        if (record.Attempts >= 5)
        {
            await _audit.WriteAsync(null, "OTP_VERIFY_BLOCKED", JsonSerializer.Serialize(new { identifier, reason = "TOO_MANY_ATTEMPTS" }), ip, userAgent, ct);
            throw new UnauthorizedAccessException("Too many attempts. Try again later.");
        }

        var ok = _hasher.Verify(identifier, otp, record.CodeHash);
        if (!ok)
        {
            await _otps.IncrementAttemptsAsync(record.OtpId, ct);
            await _audit.WriteAsync(null, "OTP_VERIFY_FAILED", JsonSerializer.Serialize(new { identifier, reason = "WRONG_OTP" }), ip, userAgent, ct);
            throw new UnauthorizedAccessException("Invalid or expired OTP.");
        }

        // Create user if not exists
        var existing = await _users.GetByPhoneOrEmailAsync(identifier, ct);
        var user = existing ?? await _users.CreateAsync(BuildNewUser(identifier, role, request.FirstName, request.FullName), ct);

        var (token, expiresIn) = _jwt.CreateAccessToken(user.UserId, user.Role, user.Tier);

        await _audit.WriteAsync(user.UserId, "LOGIN_SUCCESS", JsonSerializer.Serialize(new { user.UserId, user.Role, user.Tier }), ip, userAgent, ct);

        return new AuthVerifyResponse(token, "Bearer", expiresIn);
    }

    private static User BuildNewUser(string identifier, string role, string? firstName, string? fullName)
    {
        var isEmail = identifier.Contains('@');

        return new User
        {
            UserId = Guid.NewGuid(),
            Role = role,
            Tier = role == "Volunteer" ? 1 : 1,
            FirstName = firstName,
            FullName = fullName,
            Email = isEmail ? identifier : null,
            Phone = isEmail ? null : identifier,
            Status = "Active",
            CreatedAt = DateTime.UtcNow
        };
    }
}