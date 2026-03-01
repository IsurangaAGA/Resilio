using Resilio.Core.DTOs;

namespace Resilio.Core.Interfaces;

public interface IAuthService
{
    Task<AuthStartResponse> StartAsync(AuthStartRequest request, string? ip, string? userAgent, CancellationToken ct);
    Task<AuthVerifyResponse> VerifyAsync(AuthVerifyRequest request, string? ip, string? userAgent, CancellationToken ct);

    Task<AuthRefreshResponse> RefreshAsync(AuthRefreshRequest request, string? ip, string? userAgent, CancellationToken ct);
    Task LogoutAsync(AuthLogoutRequest request, string? ip, string? userAgent, CancellationToken ct);
}