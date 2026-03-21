using Resilio.Core.DTOs;
using Resilio.Core.Interfaces;

namespace Resilio.API.Services;

public sealed class ReliefRequestService : IReliefRequestService
{
    private readonly IReliefRequestRepository _repo;

    private static readonly HashSet<string> ValidUrgencies =
        new(StringComparer.OrdinalIgnoreCase)
        { "Low", "Medium", "High", "Critical" };

    public ReliefRequestService(IReliefRequestRepository repo) => _repo = repo;

    //create a relief request
    public async Task<ReliefRequestResponse> CreateAsync(
        Guid userId, ReliefRequestCreateRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Area))
            throw new ArgumentException("Area is required.");

        if (!ValidUrgencies.Contains(req.Urgency ?? ""))
            throw new ArgumentException(
                "Urgency must be Low, Medium, High, or Critical.");

        var record = new ReliefRequestRecord(
            RequestId:       Guid.NewGuid(),
            CreatedByUserId: userId,
            Area:            req.Area.Trim(),
            Description:     req.Description?.Trim(),
            Urgency:         req.Urgency!,
            Status:          "Open",
            CreatedAt:       DateTime.UtcNow,
            UpdatedAt:       DateTime.UtcNow);

        var created = await _repo.CreateAsync(record, ct);
        return ToResponse(created);
    }

    // view
    private static readonly HashSet<string> ValidStatuses =
        new(StringComparer.OrdinalIgnoreCase)
        { "Open", "Assigned", "Completed" };

    public async Task<IReadOnlyList<ReliefRequestResponse>> GetAllAsync(
        string? statusFilter, CancellationToken ct)
{
        if (!string.IsNullOrWhiteSpace(statusFilter) &&
            !ValidStatuses.Contains(statusFilter))
            throw new ArgumentException(
                "Status filter must be Open, Assigned, or Completed.");

        var records = await _repo.GetAllAsync(statusFilter, ct);
        return records.Select(ToResponse).ToList();
}

    //edit
    public async Task<ReliefRequestResponse> UpdateAsync(
    Guid id, ReliefRequestUpdateRequest req, CancellationToken ct)
{
    // fetch existing record
    var existing = await _repo.GetByIdAsync(id, ct)
        ?? throw new KeyNotFoundException("Relief request not found.");

    // can't edit a Completed request
    if (existing.Status == "Completed")
        throw new InvalidOperationException(
            "Cannot edit a Completed relief request.");

    // validate urgency if provided
    if (req.Urgency is not null && !ValidUrgencies.Contains(req.Urgency))
        throw new ArgumentException(
            "Urgency must be Low, Medium, High, or Critical.");

    // only update fields that were sent
    var updated = existing with {
        Area        = req.Area?.Trim()        ?? existing.Area,
        Description = req.Description?.Trim() ?? existing.Description,
        Urgency     = req.Urgency             ?? existing.Urgency,
        UpdatedAt   = DateTime.UtcNow
    };

    return ToResponse(await _repo.UpdateAsync(updated, ct));
}
    // public Task DeleteAsync(Guid requestId, CancellationToken ct)
    //     => throw new NotImplementedException();

    // public Task<byte[]> ExportToPdfAsync(
    //     string? statusFilter, string? dateFrom, string? dateTo, CancellationToken ct)
    //     => throw new NotImplementedException();

    private static ReliefRequestResponse ToResponse(ReliefRequestRecord r) => new(
        r.RequestId, r.CreatedByUserId, r.Area, r.Description,
        r.Urgency, r.Status, r.CreatedAt, r.UpdatedAt);
}