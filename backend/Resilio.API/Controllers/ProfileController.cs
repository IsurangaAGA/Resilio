using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resilio.Core.DTOs;
using Resilio.Core.Interfaces;

namespace Resilio.API.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize] // Must be logged in
public sealed class ProfileController : ControllerBase
{
    private readonly IProfileService _profiles;

    public ProfileController(IProfileService profiles) => _profiles = profiles;

    [HttpGet("me")]
    public async Task<ActionResult<ProfileMeResponse>> GetMe(CancellationToken ct)
    {
        var (userId, role) = GetAuthContext();

        var result = await _profiles.GetMeAsync(userId, role, ct);
        return Ok(result);
    }

    [HttpPut("me")]
    public async Task<ActionResult<ProfileMeResponse>> UpdateMe([FromBody] ProfileUpdateRequest request, CancellationToken ct)
    {
        var (userId, role) = GetAuthContext();

        try
        {
            var result = await _profiles.UpdateMeAsync(userId, role, request, ct);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails { Title = "Validation error", Detail = ex.Message });
        }
    }

    private (Guid UserId, string Role) GetAuthContext()
    {
        // userId stored in JWT as "sub"
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(sub, out var userId))
            throw new UnauthorizedAccessException("Invalid token subject.");

        var role = User.FindFirstValue(ClaimTypes.Role);
        if (string.IsNullOrWhiteSpace(role))
            throw new UnauthorizedAccessException("Missing role.");

        return (userId, role);
    }
}