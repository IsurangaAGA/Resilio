using Microsoft.Data.SqlClient;
using Resilio.Core.Interfaces;

namespace Resilio.Infrastructure.Repositories;

public sealed class OtpRepository : IOtpRepository
{
    private readonly IDbConnectionFactory _factory;

    public OtpRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task InsertAsync(string identifier, string channel, string codeHash, DateTime expiresAt, CancellationToken ct)
    {
        const string sql = @"
INSERT INTO OtpRequests (Identifier, Channel, CodeHash, ExpiresAt, Attempts, SentCount, LastSentAt)
VALUES (@Identifier, @Channel, @CodeHash, @ExpiresAt, 0, 1, SYSDATETIME());";

        using var conn = (SqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(ct);

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Identifier", identifier);
        cmd.Parameters.AddWithValue("@Channel", channel);
        cmd.Parameters.AddWithValue("@CodeHash", codeHash);
        cmd.Parameters.AddWithValue("@ExpiresAt", expiresAt);

        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<OtpRecord?> GetLatestValidAsync(string identifier, DateTime nowUtc, CancellationToken ct)
    {
        const string sql = @"
SELECT TOP 1 OtpId, Identifier, CodeHash, ExpiresAt, Attempts
FROM OtpRequests
WHERE Identifier = @Identifier AND ExpiresAt > @NowUtc
ORDER BY CreatedAt DESC;";

        using var conn = (SqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(ct);

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Identifier", identifier);
        cmd.Parameters.AddWithValue("@NowUtc", nowUtc);

        using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;

        return new OtpRecord(
            OtpId: reader.GetInt64(0),
            Identifier: reader.GetString(1),
            CodeHash: reader.GetString(2),
            ExpiresAt: reader.GetDateTime(3),
            Attempts: reader.GetInt32(4)
        );
    }

    public async Task IncrementAttemptsAsync(long otpId, CancellationToken ct)
    {
        const string sql = @"UPDATE OtpRequests SET Attempts = Attempts + 1 WHERE OtpId = @OtpId;";

        using var conn = (SqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(ct);

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@OtpId", otpId);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}