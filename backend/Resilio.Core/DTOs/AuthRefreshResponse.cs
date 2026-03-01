namespace Resilio.Core.DTOs;

public sealed record AuthRefreshResponse(
    string AccessToken,
    string RefreshToken,
    string TokenType,
    int ExpiresInSeconds
);