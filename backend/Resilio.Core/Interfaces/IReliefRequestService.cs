// Resilio.Core/Interfaces/IReliefRequestService.cs
using Resilio.Core.DTOs;

namespace Resilio.Core.Interfaces;

public interface IReliefRequestService
{
    Task<ReliefRequestResponse> CreateAsync(
        Guid userId, ReliefRequestCreateRequest request, CancellationToken ct);

    Task<IReadOnlyList<ReliefRequestResponse>> GetAllAsync(
        string? statusFilter, CancellationToken ct);

    // Task<ReliefRequestResponse> UpdateAsync(
    //     Guid requestId, ReliefRequestUpdateRequest request, CancellationToken ct);

    // Task DeleteAsync(Guid requestId, CancellationToken ct);

    // Task<byte[]> ExportToPdfAsync(
    //     string? statusFilter, string? dateFrom, string? dateTo,
    //     CancellationToken ct);
}