using System.Text.Json;
using Resilio.Core.DTOs;
using Resilio.Core.Interfaces;

namespace Resilio.API.Services;

public sealed class ProfileService : IProfileService
{
    private readonly IVictimProfileRepository _victims;
    private readonly IVolunteerProfileRepository _volunteers;

    public ProfileService(IVictimProfileRepository victims, IVolunteerProfileRepository volunteers)
    {
        _victims = victims;
        _volunteers = volunteers;
    }

    public async Task<ProfileMeResponse> GetMeAsync(Guid userId, string role, CancellationToken ct)
    {
        role = NormalizeRole(role);

        if (role == "Victim")
        {
            var record = await _victims.GetAsync(userId, ct);
            return new ProfileMeResponse(
                Role: role,
                Victim: MapVictim(record),
                Volunteer: null
            );
        }

        if (role == "Volunteer")
        {
            var record = await _volunteers.GetAsync(userId, ct);
            return new ProfileMeResponse(
                Role: role,
                Victim: null,
                Volunteer: MapVolunteer(record)
            );
        }

        throw new UnauthorizedAccessException("Unknown role.");
    }

    public async Task<ProfileMeResponse> UpdateMeAsync(Guid userId, string role, ProfileUpdateRequest request, CancellationToken ct)
    {
        role = NormalizeRole(role);

        if (role == "Victim")
        {
            // Role enforcement: victim can only use victim fields
            if (request.Skills is not null || request.Availability is not null)
                throw new ArgumentException("Victim cannot update volunteer fields.");

            var categories = request.HelpCategories ?? new List<string>();
            ValidateList(categories, "HelpCategories", maxItems: 10);

            var json = JsonSerializer.Serialize(categories);

            await _victims.UpsertAsync(
                new VictimProfileRecord(userId, request.LocationText?.Trim(), json),
                ct);

            var updated = await _victims.GetAsync(userId, ct);
            return new ProfileMeResponse("Victim", MapVictim(updated), null);
        }

        if (role == "Volunteer")
        {
            // Role enforcement: volunteer can only use volunteer fields
            if (request.HelpCategories is not null)
                throw new ArgumentException("Volunteer cannot update victim fields.");

            var skills = request.Skills ?? new List<string>();
            ValidateList(skills, "Skills", maxItems: 20);

            var json = JsonSerializer.Serialize(skills);

            await _volunteers.UpsertAsync(
                new VolunteerProfileRecord(userId, request.LocationText?.Trim(), json, request.Availability?.Trim()),
                ct);

            var updated = await _volunteers.GetAsync(userId, ct);
            return new ProfileMeResponse("Volunteer", null, MapVolunteer(updated));
        }

        throw new UnauthorizedAccessException("Unknown role.");
    }

    private static string NormalizeRole(string role)
    {
        role = role.Trim();
        return role switch
        {
            "Victim" => "Victim",
            "Volunteer" => "Volunteer",
            _ => role
        };
    }

    private static void ValidateList(List<string> items, string field, int maxItems)
    {
        if (items.Count > maxItems)
            throw new ArgumentException($"{field} exceeds max items {maxItems}.");

        foreach (var s in items)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new ArgumentException($"{field} contains empty item.");
            if (s.Length > 50)
                throw new ArgumentException($"{field} item too long (max 50).");
        }
    }

    private static VictimProfileDto MapVictim(VictimProfileRecord? record)
    {
        if (record is null)
            return new VictimProfileDto(null, new List<string>());

        var list = SafeDeserializeList(record.HelpCategoriesJson);
        return new VictimProfileDto(record.LocationText, list);
    }

    private static VolunteerProfileDto MapVolunteer(VolunteerProfileRecord? record)
    {
        if (record is null)
            return new VolunteerProfileDto(null, new List<string>(), null);

        var list = SafeDeserializeList(record.SkillsJson);
        return new VolunteerProfileDto(record.LocationText, list, record.Availability);
    }

    private static List<string> SafeDeserializeList(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new List<string>();
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }
}