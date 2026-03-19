// Resilio.Infrastructure/Repositories/ReliefRequestRepository.cs
using Microsoft.Data.SqlClient;
using Resilio.Core.Interfaces;

namespace Resilio.Infrastructure.Repositories;

public sealed class ReliefRequestRepository : IReliefRequestRepository
{
    private readonly IDbConnectionFactory _factory;

    public ReliefRequestRepository(IDbConnectionFactory f) => _factory = f;
    //create
    public async Task<ReliefRequestRecord> CreateAsync(
        ReliefRequestRecord record, CancellationToken ct)
    {
        const string sql = @"
            INSERT INTO dbo.ReliefRequests
                (RequestId, CreatedByUserId, Area, Description, Urgency, Status, CreatedAt, UpdatedAt)
            VALUES
                (@Id, @UserId, @Area, @Desc, @Urgency, @Status, @CreatedAt, @UpdatedAt);";

        using var conn = (SqlConnection)_factory.CreateConnection();
        await conn.OpenAsync(ct);
        using var cmd = new SqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@Id",        record.RequestId);
        cmd.Parameters.AddWithValue("@UserId",    record.CreatedByUserId);
        cmd.Parameters.AddWithValue("@Area",      record.Area);
        cmd.Parameters.AddWithValue("@Desc",      (object?)record.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Urgency",   record.Urgency);
        cmd.Parameters.AddWithValue("@Status",    record.Status);
        cmd.Parameters.AddWithValue("@CreatedAt", record.CreatedAt);
        cmd.Parameters.AddWithValue("@UpdatedAt", record.UpdatedAt);

        await cmd.ExecuteNonQueryAsync(ct);
        return record;
    }

    // for now its not implemented
    public async Task<IReadOnlyList<ReliefRequestRecord>> GetAllAsync(
    string? statusFilter, CancellationToken ct)
{
    var sql = "SELECT RequestId, CreatedByUserId, Area, Description, " +
              "Urgency, Status, CreatedAt, UpdatedAt " +
              "FROM dbo.ReliefRequests";

    if (!string.IsNullOrWhiteSpace(statusFilter))
        sql += " WHERE Status = @Status";

    sql += " ORDER BY CreatedAt DESC;";

    using var conn = (SqlConnection)_factory.CreateConnection();
    await conn.OpenAsync(ct);
    using var cmd = new SqlCommand(sql, conn);

    if (!string.IsNullOrWhiteSpace(statusFilter))
        cmd.Parameters.AddWithValue("@Status", statusFilter);

    using var reader = await cmd.ExecuteReaderAsync(ct);
    var list = new List<ReliefRequestRecord>();
    while (await reader.ReadAsync(ct))
        list.Add(MapRow(reader));

    return list;
}

    public async Task<ReliefRequestRecord?> GetByIdAsync(
    Guid requestId, CancellationToken ct)
{
    const string sql =
        "SELECT RequestId, CreatedByUserId, Area, Description, " +
        "Urgency, Status, CreatedAt, UpdatedAt " +
        "FROM dbo.ReliefRequests WHERE RequestId = @Id;";

    using var conn = (SqlConnection)_factory.CreateConnection();
    await conn.OpenAsync(ct);
    using var cmd = new SqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("@Id", requestId);

    using var reader = await cmd.ExecuteReaderAsync(ct);
    if (!await reader.ReadAsync(ct)) return null;
    return MapRow(reader);
}

    public Task<ReliefRequestRecord> UpdateAsync(
        ReliefRequestRecord record, CancellationToken ct)
        => throw new NotImplementedException();

    public Task DeleteAsync(Guid requestId, CancellationToken ct)
        => throw new NotImplementedException();

    private static ReliefRequestRecord MapRow(SqlDataReader r) => new(
        RequestId:       r.GetGuid(0),
        CreatedByUserId: r.GetGuid(1),
        Area:            r.GetString(2),
        Description:     r.IsDBNull(3) ? null : r.GetString(3),
        Urgency:         r.GetString(4),
        Status:          r.GetString(5),
        CreatedAt:       r.GetDateTime(6),
        UpdatedAt:       r.GetDateTime(7)
);
}