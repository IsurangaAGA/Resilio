using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resilio.Core.DTOs;
using Resilio.Core.Interfaces;

namespace Resilio.API.Controllers;

[ApiController]
[Route("api/relief-requests")]
[Authorize]
public sealed class ReliefRequestsController : ControllerBase
{
    private readonly IReliefRequestService _service;

    public ReliefRequestsController(IReliefRequestService s) => _service = s;

    [HttpPost]
    public async Task<ActionResult<ReliefRequestResponse>> Create(
        [FromBody] ReliefRequestCreateRequest request, CancellationToken ct)
    {
        var userId = GetUserId();
        try
        {
            var result = await _service.CreateAsync(userId, request, ct);
            return CreatedAtAction(nameof(Create), new { }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title  = "Validation error",
                Detail = ex.Message
            });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ReliefRequestResponse>>> GetAll(
        [FromQuery] string? status, CancellationToken ct)
    {
        try
        {
            return Ok(await _service.GetAllAsync(status, ct));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title  = "Validation error",
                Detail = ex.Message
            });
        }
    }
    private Guid GetUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(sub, out var userId))
            throw new UnauthorizedAccessException("Invalid token.");
        return userId;
    }
}