using Resilio.Core.DTOs;

namespace Resilio.Core.Interfaces;

public interface IProfileService
{
    Task<ProfileMeResponse> GetMeAsync(Guid userId, string role, CancellationToken ct);
    Task<ProfileMeResponse> UpdateMeAsync(Guid userId, string role, ProfileUpdateRequest request, CancellationToken ct);
}