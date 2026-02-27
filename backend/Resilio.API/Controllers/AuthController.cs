using Microsoft.AspNetCore.Mvc;
using Resilio.Core.DTOs;
using Resilio.Core.Interfaces;

namespace Resilio.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("start")]
    public async Task<ActionResult<AuthStartResponse>> Start([FromBody] AuthStartRequest request, CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var ua = Request.Headers.UserAgent.ToString();

        try
        {
            var result = await _auth.StartAsync(request, ip, ua, ct);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails { Title = "Validation error", Detail = ex.Message });
        }
    }

    [HttpPost("verify")]
    public async Task<ActionResult<AuthVerifyResponse>> Verify([FromBody] AuthVerifyRequest request, CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var ua = Request.Headers.UserAgent.ToString();

        try
        {
            var result = await _auth.VerifyAsync(request, ip, ua, ct);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails { Title = "Validation error", Detail = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new ProblemDetails { Title = "Unauthorized", Detail = "Invalid or expired OTP." });
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthRefreshResponse>> Refresh([FromBody] AuthRefreshRequest request, CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var ua = Request.Headers.UserAgent.ToString();

        try
        {
            var result = await _auth.RefreshAsync(request, ip, ua, ct);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails { Title = "Validation error", Detail = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new ProblemDetails { Title = "Unauthorized", Detail = "Invalid refresh token." });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] AuthLogoutRequest request, CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var ua = Request.Headers.UserAgent.ToString();

        try
        {
            await _auth.LogoutAsync(request, ip, ua, ct);
            return Ok(new { message = "Logged out." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails { Title = "Validation error", Detail = ex.Message });
        }
    }
}