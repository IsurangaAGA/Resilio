namespace Resilio.Core.DTOs;

public sealed record AuthStartRequest(
    string Identifier,   // phone or email
    string Channel,      // "SMS" or "EMAIL"
    string Role          // "Victim" or "Volunteer"
);