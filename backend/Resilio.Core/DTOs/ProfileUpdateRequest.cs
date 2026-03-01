namespace Resilio.Core.DTOs;

public sealed record ProfileUpdateRequest(
    // Victim fields
    string? LocationText,
    List<string>? HelpCategories,

    // Volunteer fields
    List<string>? Skills,
    string? Availability
);