using System.Data;
using Microsoft.Data.SqlClient;
using Resilio.Core.Interfaces;
using Resilio.Core.Models;

namespace Resilio.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _factory;

    public UserRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<User?> GetByPhoneOrEmailAsync(string identifier, CancellationToken ct)
    {
        const string sql = @"
SELECT TOP 1 UserId, Role, Tier, FirstName, FullName, Phone, Email, Status, CreatedAt
FROM Users
WHERE Phone = @id OR Email = @id
ORDER BY CreatedAt DESC;";

        using var conn = _factory.CreateConnection();
        await ((SqlConnection)conn).OpenAsync(ct);

        using var cmd = new SqlCommand(sql, (SqlConnection)conn);
        cmd.Parameters.AddWithValue("@id", identifier);

        using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;

        return new User
        {
            UserId = reader.GetGuid(0),
            Role = reader.GetString(1),
            Tier = reader.GetInt32(2),
            FirstName = reader.IsDBNull(3) ? null : reader.GetString(3),
            FullName = reader.IsDBNull(4) ? null : reader.GetString(4),
            Phone = reader.IsDBNull(5) ? null : reader.GetString(5),
            Email = reader.IsDBNull(6) ? null : reader.GetString(6),
            Status = reader.GetString(7),
            CreatedAt = reader.GetDateTime(8)
        };
    }

    public async Task<User> CreateAsync(User user, CancellationToken ct)
    {
        const string sql = @"
INSERT INTO Users (UserId, Role, Tier, FirstName, FullName, Phone, Email, Status)
VALUES (@UserId, @Role, @Tier, @FirstName, @FullName, @Phone, @Email, @Status);";

        using var conn = _factory.CreateConnection();
        await ((SqlConnection)conn).OpenAsync(ct);

        using var cmd = new SqlCommand(sql, (SqlConnection)conn);
        cmd.Parameters.AddWithValue("@UserId", user.UserId);
        cmd.Parameters.AddWithValue("@Role", user.Role);
        cmd.Parameters.AddWithValue("@Tier", user.Tier);
        cmd.Parameters.AddWithValue("@FirstName", (object?)user.FirstName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@FullName", (object?)user.FullName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Phone", (object?)user.Phone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Email", (object?)user.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Status", user.Status);

        await cmd.ExecuteNonQueryAsync(ct);
        return user;
    }

    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken ct)
{
    const string sql = @"
SELECT TOP 1 UserId, Role, Tier, FirstName, FullName, Phone, Email, Status, CreatedAt
FROM Users
WHERE UserId = @UserId;";

    using var conn = _factory.CreateConnection();
    await ((SqlConnection)conn).OpenAsync(ct);

    using var cmd = new SqlCommand(sql, (SqlConnection)conn);
    cmd.Parameters.AddWithValue("@UserId", userId);

    using var reader = await cmd.ExecuteReaderAsync(ct);
    if (!await reader.ReadAsync(ct)) return null;

    return new User
    {
        UserId = reader.GetGuid(0),
        Role = reader.GetString(1),
        Tier = reader.GetInt32(2),
        FirstName = reader.IsDBNull(3) ? null : reader.GetString(3),
        FullName = reader.IsDBNull(4) ? null : reader.GetString(4),
        Phone = reader.IsDBNull(5) ? null : reader.GetString(5),
        Email = reader.IsDBNull(6) ? null : reader.GetString(6),
        Status = reader.GetString(7),
        CreatedAt = reader.GetDateTime(8)
    };
}
}