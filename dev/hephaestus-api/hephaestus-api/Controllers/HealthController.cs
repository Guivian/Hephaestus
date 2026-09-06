using Hephaestus.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace Hephaestus.Api.Controllers;
[ApiController]
[Route("api/health")]
public sealed class HealthController(HephaestusDbContext database) : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "ok" });

    [HttpGet("database")]
    public async Task<IActionResult> Database(CancellationToken cancellationToken)
    {
        var connected = await database.Database.CanConnectAsync(cancellationToken);

        return connected ? Ok(new { status = "ok", database = "connected" }) : StatusCode(503, new { status = "error", database = "unavailable" });
    }
}