namespace Resilio.Core.DTOs;

public sealed record AuthVerifyRequest(
    string Identifier,
    string Otp,
    string Role,
    string? FirstName,
    string? FullName
);