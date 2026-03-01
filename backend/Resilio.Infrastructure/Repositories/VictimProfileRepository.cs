using Microsoft.Data.SqlClient;
using Resilio.Core.Interfaces;

namespace Resilio.Infrastructure.Repositories;

public sealed class VictimProfileRepository : IVictimProfileRepository
{
    private readonly IDbConnectionFactory _factory;

    public VictimProfileRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<VictimProfileRecord?> GetAsync(Guid userId, CancellationToken ct)
    {
        const string sql = @"
SELECT TOP 1 UserId, LocationText, HelpCategoriesJson
FROM VictimProfiles
WHERE UserId = @UserId;";

        using var conn = (SqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(ct);

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@UserId", userId);

        using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;

        return new VictimProfileRecord(
            UserId: reader.GetGuid(0),
            LocationText: reader.IsDBNull(1) ? null : reader.GetString(1),
            HelpCategoriesJson: reader.IsDBNull(2) ? null : reader.GetString(2)
        );
    }

    public async Task UpsertAsync(VictimProfileRecord record, CancellationToken ct)
    {
        // MERGE = Upsert (insert if missing, else update)
        const string sql = @"
MERGE VictimProfiles AS target
USING (SELECT @UserId AS UserId) AS source
ON target.UserId = source.UserId
WHEN MATCHED THEN
    UPDATE SET LocationText = @LocationText,
               HelpCategoriesJson = @HelpCategoriesJson,
               UpdatedAt = SYSDATETIME()
WHEN NOT MATCHED THEN
    INSERT (UserId, LocationText, HelpCategoriesJson)
    VALUES (@UserId, @LocationText, @HelpCategoriesJson);";

        using var conn = (SqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(ct);

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@UserId", record.UserId);
        cmd.Parameters.AddWithValue("@LocationText", (object?)record.LocationText ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@HelpCategoriesJson", (object?)record.HelpCategoriesJson ?? DBNull.Value);

        await cmd.ExecuteNonQueryAsync(ct);
    }
}