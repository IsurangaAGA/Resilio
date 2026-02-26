namespace Resilio.Core.DTOs;

public sealed record AuthVerifyResponse(
    string AccessToken,
    string TokenType,
    int ExpiresInSeconds
);