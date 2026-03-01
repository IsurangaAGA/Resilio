namespace Resilio.Core.DTOs;

public sealed record ProfileMeResponse(
    string Role,
    VictimProfileDto? Victim,
    VolunteerProfileDto? Volunteer
);

public sealed record VictimProfileDto(
    string? LocationText,
    List<string> HelpCategories
);

public sealed record VolunteerProfileDto(
    string? LocationText,
    List<string> Skills,
    string? Availability
);