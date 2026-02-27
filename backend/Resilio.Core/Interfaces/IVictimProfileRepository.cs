namespace Resilio.Core.Interfaces;

public sealed record VictimProfileRecord(
    Guid UserId,
    string? LocationText,
    string? HelpCategoriesJson
);

public interface IVictimProfileRepository
{
    Task<VictimProfileRecord?> GetAsync(Guid userId, CancellationToken ct);
    Task UpsertAsync(VictimProfileRecord record, CancellationToken ct);
}