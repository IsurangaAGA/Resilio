using Resilio.Core.DTOs;

namespace Resilio.Core.Interfaces;

public interface IReliefRequestService
{
    Task<ReliefRequestResponse> CreateAsync(
        Guid userId, ReliefRequestCreateRequest request, CancellationToken ct);

    Task<IReadOnlyList<ReliefRequestResponse>> GetAllAsync(
        string? statusFilter, CancellationToken ct);

    Task<ReliefRequestResponse> UpdateAsync(
        Guid requestId, ReliefRequestUpdateRequest request, CancellationToken ct);

    Task DeleteAsync(Guid requestId, CancellationToken ct);

    Task<IReadOnlyList<ReliefRequestResponse>> GetByUserAsync(
    Guid userId, CancellationToken ct);

}