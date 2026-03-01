namespace Resilio.Core.Models;

public sealed class User
{
    public Guid UserId { get; init; }
    public string Role { get; init; } = default!;
    public int Tier { get; init; } = 1;
    public string? FirstName { get; init; }
    public string? FullName { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string Status { get; init; } = "Active";
    public DateTime CreatedAt { get; init; }
}