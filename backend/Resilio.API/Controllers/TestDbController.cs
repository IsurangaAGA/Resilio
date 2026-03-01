using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Resilio.Core.Interfaces;

namespace Resilio.API.Controllers;

[ApiController]
[Route("api/test-db")]
public sealed class TestDbController : ControllerBase
{
    private readonly IDbConnectionFactory _factory;

    public TestDbController(IDbConnectionFactory factory) => _factory = factory;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        try
        {
            using var conn = (SqlConnection)_factory.CreateConnection();
            await conn.OpenAsync(ct);

            using var cmd = new SqlCommand("SELECT 1;", conn);
            var result = await cmd.ExecuteScalarAsync(ct);

            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { ok = false, error = ex.Message });
        }
    }
}