using Microsoft.Data.SqlClient;
using Resilio.Core.Interfaces;

namespace Resilio.Infrastructure.Repositories;

public sealed class AuditLogRepository : IAuditLogRepository
{
    private readonly IDbConnectionFactory _factory;

    public AuditLogRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task WriteAsync(Guid? userId, string action, string? metadataJson, string? ip, string? userAgent, CancellationToken ct)
    {
        const string sql = @"
INSERT INTO AuditLogs (UserId, Action, MetadataJson, Ip, UserAgent)
VALUES (@UserId, @Action, @MetadataJson, @Ip, @UserAgent);";

        using var conn = (SqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(ct);

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@UserId", (object?)userId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Action", action);
        cmd.Parameters.AddWithValue("@MetadataJson", (object?)metadataJson ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Ip", (object?)ip ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@UserAgent", (object?)userAgent ?? DBNull.Value);

        await cmd.ExecuteNonQueryAsync(ct);
    }
}