namespace Resilio.Core.Interfaces;

public sealed record VolunteerProfileRecord(
    Guid UserId,
    string? LocationText,
    string? SkillsJson,
    string? Availability
);

public interface IVolunteerProfileRepository
{
    Task<VolunteerProfileRecord?> GetAsync(Guid userId, CancellationToken ct);
    Task UpsertAsync(VolunteerProfileRecord record, CancellationToken ct);
}