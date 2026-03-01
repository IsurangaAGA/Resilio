using Microsoft.Data.SqlClient;
using Resilio.Core.Interfaces;

namespace Resilio.Infrastructure.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IDbConnectionFactory _factory;

    public RefreshTokenRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task InsertAsync(Guid userId, string tokenHash, DateTime expiresAtUtc, CancellationToken ct)
    {
        const string sql = @"
INSERT INTO RefreshTokens (UserId, TokenHash, ExpiresAt)
VALUES (@UserId, @TokenHash, @ExpiresAt);";

        using var conn = (SqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(ct);

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@UserId", userId);
        cmd.Parameters.AddWithValue("@TokenHash", tokenHash);
        cmd.Parameters.AddWithValue("@ExpiresAt", expiresAtUtc);

        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<RefreshTokenRecord?> GetByHashAsync(string tokenHash, CancellationToken ct)
    {
        const string sql = @"
SELECT TOP 1 TokenId, UserId, TokenHash, ExpiresAt, RevokedAt, ReplacedByTokenHash
FROM RefreshTokens
WHERE TokenHash = @TokenHash;";

        using var conn = (SqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(ct);

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@TokenHash", tokenHash);

        using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;

        return new RefreshTokenRecord(
            TokenId: reader.GetInt64(0),
            UserId: reader.GetGuid(1),
            TokenHash: reader.GetString(2),
            ExpiresAt: reader.GetDateTime(3),
            RevokedAt: reader.IsDBNull(4) ? null : reader.GetDateTime(4),
            ReplacedByTokenHash: reader.IsDBNull(5) ? null : reader.GetString(5)
        );
    }

    public async Task RevokeAsync(long tokenId, DateTime revokedAtUtc, string? replacedByHash, CancellationToken ct)
    {
        const string sql = @"
UPDATE RefreshTokens
SET RevokedAt = @RevokedAt,
    ReplacedByTokenHash = @ReplacedBy
WHERE TokenId = @TokenId;";

        using var conn = (SqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(ct);

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@RevokedAt", revokedAtUtc);
        cmd.Parameters.AddWithValue("@ReplacedBy", (object?)replacedByHash ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@TokenId", tokenId);

        await cmd.ExecuteNonQueryAsync(ct);
    }
}