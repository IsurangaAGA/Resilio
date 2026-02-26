namespace Resilio.Core.Interfaces;

public sealed record OtpRecord(
    long OtpId,
    string Identifier,
    string CodeHash,
    DateTime ExpiresAt,
    int Attempts
);

public interface IOtpRepository
{
    Task InsertAsync(string identifier, string channel, string codeHash, DateTime expiresAt, CancellationToken ct);
    Task<OtpRecord?> GetLatestValidAsync(string identifier, DateTime nowUtc, CancellationToken ct);
    Task IncrementAttemptsAsync(long otpId, CancellationToken ct);
}