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

    private readonly IOtpHasher _otpHasher;
    private readonly IOtpGenerator _otpGenerator;

    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly ITokenHasher _tokenHasher;
    private readonly IRefreshTokenGenerator _refreshGenerator;

    private readonly int _otpExpiryMinutes;
    private readonly int _refreshDays;

    private readonly IEmailSender _emailSender;

    public AuthService(
        IUserRepository users,
        IOtpRepository otps,
        IAuditLogRepository audit,
        IJwtTokenService jwt,
        IOtpHasher otpHasher,
        IOtpGenerator otpGenerator,
        IEmailSender emailSender,
        IRefreshTokenRepository refreshRepo,
        ITokenHasher tokenHasher,
        IRefreshTokenGenerator refreshGenerator,
        IConfiguration config)
    {
        _users = users;
        _otps = otps;
        _audit = audit;
        _jwt = jwt;

        _otpHasher = otpHasher;
        _otpGenerator = otpGenerator;

        _refreshRepo = refreshRepo;
        _tokenHasher = tokenHasher;
        _refreshGenerator = refreshGenerator;

        _emailSender = emailSender;

        _otpExpiryMinutes = int.TryParse(config["Otp:ExpiryMinutes"], out var m) ? m : 5;
        _refreshDays = int.TryParse(config["Auth:RefreshTokenDays"], out var d) ? d : 14;
    }

    public async Task<AuthStartResponse> StartAsync(AuthStartRequest request, string? ip, string? userAgent, CancellationToken ct)
    {
        var identifier = request.Identifier.Trim();
        var channel = request.Channel.Trim().ToUpperInvariant();
        var role = request.Role.Trim();

        if (string.IsNullOrWhiteSpace(identifier)) throw new ArgumentException("Identifier is required.");
        if (channel is not ("SMS" or "EMAIL")) throw new ArgumentException("Channel must be SMS or EMAIL.");
        if (role is not ("Victim" or "Volunteer")) throw new ArgumentException("Role must be Victim or Volunteer.");

        var otp = _otpGenerator.Generate6Digits();
        var hash = _otpHasher.Hash(identifier, otp);
        var expiresAt = DateTime.UtcNow.AddMinutes(_otpExpiryMinutes);

        await _otps.InsertAsync(identifier, channel, hash, expiresAt, ct);

        await _audit.WriteAsync(
            userId: null,
            action: "OTP_SENT",
            metadataJson: JsonSerializer.Serialize(new { identifier, channel, role, expiresAt }),
            ip: ip,
            userAgent: userAgent,
            ct: ct);

        if (channel == "EMAIL" && identifier.Contains("@"))
{
    await _emailSender.SendOtpAsync(identifier, otp, ct);
}

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

        if (record.Attempts >= 5)
        {
            await _audit.WriteAsync(null, "OTP_VERIFY_BLOCKED", JsonSerializer.Serialize(new { identifier, reason = "TOO_MANY_ATTEMPTS" }), ip, userAgent, ct);
            throw new UnauthorizedAccessException("Too many attempts. Try again later.");
        }

        var ok = _otpHasher.Verify(identifier, otp, record.CodeHash);
        if (!ok)
        {
            await _otps.IncrementAttemptsAsync(record.OtpId, ct);
            await _audit.WriteAsync(null, "OTP_VERIFY_FAILED", JsonSerializer.Serialize(new { identifier, reason = "WRONG_OTP" }), ip, userAgent, ct);
            throw new UnauthorizedAccessException("Invalid or expired OTP.");
        }

        // Create user if not exists
        var existing = await _users.GetByPhoneOrEmailAsync(identifier, ct);
        var user = existing ?? await _users.CreateAsync(BuildNewUser(identifier, role, request.FirstName, request.FullName), ct);

        // Issue JWT
        var (accessToken, expiresIn) = _jwt.CreateAccessToken(user.UserId, user.Role, user.Tier);

        // Issue refresh token and store hash in DB
        var refreshToken = _refreshGenerator.Generate();
        var refreshHash = _tokenHasher.Hash(refreshToken);
        var refreshExpiry = DateTime.UtcNow.AddDays(_refreshDays);

        await _refreshRepo.InsertAsync(user.UserId, refreshHash, refreshExpiry, ct);

        await _audit.WriteAsync(user.UserId, "LOGIN_SUCCESS",
            JsonSerializer.Serialize(new { user.UserId, user.Role, user.Tier }),
            ip, userAgent, ct);

        // Return BOTH tokens
        return new AuthVerifyResponse(accessToken, refreshToken, "Bearer", expiresIn);
    }

    public async Task<AuthRefreshResponse> RefreshAsync(AuthRefreshRequest request, string? ip, string? userAgent, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new ArgumentException("Refresh token is required.");

        var presentedHash = _tokenHasher.Hash(request.RefreshToken);
        var record = await _refreshRepo.GetByHashAsync(presentedHash, ct);

        if (record is null || record.RevokedAt is not null || record.ExpiresAt <= DateTime.UtcNow)
        {
            await _audit.WriteAsync(record?.UserId, "REFRESH_FAILED", JsonSerializer.Serialize(new { reason = "INVALID" }), ip, userAgent, ct);
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        var user = await _users.GetByIdAsync(record.UserId, ct);
        if (user is null || user.Status != "Active")
        {
            await _audit.WriteAsync(record.UserId, "REFRESH_FAILED", JsonSerializer.Serialize(new { reason = "USER_INVALID" }), ip, userAgent, ct);
            throw new UnauthorizedAccessException("User not allowed.");
        }

        // ROTATE refresh token
        var newRefreshToken = _refreshGenerator.Generate();
        var newHash = _tokenHasher.Hash(newRefreshToken);
        var newExpiry = DateTime.UtcNow.AddDays(_refreshDays);

        await _refreshRepo.InsertAsync(user.UserId, newHash, newExpiry, ct);
        await _refreshRepo.RevokeAsync(record.TokenId, DateTime.UtcNow, newHash, ct);

        // New JWT
        var (newAccessToken, expiresIn) = _jwt.CreateAccessToken(user.UserId, user.Role, user.Tier);

        await _audit.WriteAsync(user.UserId, "REFRESH_SUCCESS", JsonSerializer.Serialize(new { rotated = true }), ip, userAgent, ct);

        return new AuthRefreshResponse(newAccessToken, newRefreshToken, "Bearer", expiresIn);
    }

    public async Task LogoutAsync(AuthLogoutRequest request, string? ip, string? userAgent, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new ArgumentException("Refresh token is required.");

        var hash = _tokenHasher.Hash(request.RefreshToken);
        var record = await _refreshRepo.GetByHashAsync(hash, ct);

        // Always return OK even if token is unknown (don’t leak validity)
        if (record is null)
        {
            await _audit.WriteAsync(null, "LOGOUT_UNKNOWN_TOKEN", null, ip, userAgent, ct);
            return;
        }

        if (record.RevokedAt is null)
            await _refreshRepo.RevokeAsync(record.TokenId, DateTime.UtcNow, null, ct);

        await _audit.WriteAsync(record.UserId, "LOGOUT_SUCCESS", null, ip, userAgent, ct);
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