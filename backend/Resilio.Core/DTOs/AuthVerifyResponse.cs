namespace Resilio.Core.DTOs;

public sealed record AuthVerifyResponse(
    string AccessToken,
    string RefreshToken,
    string TokenType,
    int ExpiresInSeconds
);