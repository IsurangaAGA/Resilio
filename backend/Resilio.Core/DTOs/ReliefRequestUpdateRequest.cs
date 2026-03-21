namespace Resilio.Core.DTOs;

public sealed record ReliefRequestUpdateRequest(
    string? Area,
    string? Description,
    string? Urgency
);