namespace Resilio.Core.Interfaces;

public sealed record RefreshTokenRecord(
    long TokenId,
    Guid UserId,
    string TokenHash,
    DateTime ExpiresAt,
    DateTime? RevokedAt,
    string? ReplacedByTokenHash
);

public interface IRefreshTokenRepository
{
    Task InsertAsync(Guid userId, string tokenHash, DateTime expiresAtUtc, CancellationToken ct);
    Task<RefreshTokenRecord?> GetByHashAsync(string tokenHash, CancellationToken ct);
    Task RevokeAsync(long tokenId, DateTime revokedAtUtc, string? replacedByHash, CancellationToken ct);
}