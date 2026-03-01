using Microsoft.Data.SqlClient;
using Resilio.Core.Interfaces;

namespace Resilio.Infrastructure.Repositories;

public sealed class VolunteerProfileRepository : IVolunteerProfileRepository
{
    private readonly IDbConnectionFactory _factory;

    public VolunteerProfileRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<VolunteerProfileRecord?> GetAsync(Guid userId, CancellationToken ct)
    {
        const string sql = @"
SELECT TOP 1 UserId, LocationText, SkillsJson, Availability
FROM VolunteerProfiles
WHERE UserId = @UserId;";

        using var conn = (SqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(ct);

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@UserId", userId);

        using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;

        return new VolunteerProfileRecord(
            UserId: reader.GetGuid(0),
            LocationText: reader.IsDBNull(1) ? null : reader.GetString(1),
            SkillsJson: reader.IsDBNull(2) ? null : reader.GetString(2),
            Availability: reader.IsDBNull(3) ? null : reader.GetString(3)
        );
    }

    public async Task UpsertAsync(VolunteerProfileRecord record, CancellationToken ct)
    {
        const string sql = @"
MERGE VolunteerProfiles AS target
USING (SELECT @UserId AS UserId) AS source
ON target.UserId = source.UserId
WHEN MATCHED THEN
    UPDATE SET LocationText = @LocationText,
               SkillsJson = @SkillsJson,
               Availability = @Availability,
               UpdatedAt = SYSDATETIME()
WHEN NOT MATCHED THEN
    INSERT (UserId, LocationText, SkillsJson, Availability)
    VALUES (@UserId, @LocationText, @SkillsJson, @Availability);";

        using var conn = (SqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(ct);

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@UserId", record.UserId);
        cmd.Parameters.AddWithValue("@LocationText", (object?)record.LocationText ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@SkillsJson", (object?)record.SkillsJson ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Availability", (object?)record.Availability ?? DBNull.Value);

        await cmd.ExecuteNonQueryAsync(ct);
    }
}