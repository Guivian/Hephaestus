using System.Security.Claims;
using Hephaestus.Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hephaestus.Api.Controllers;

[ApiController]
[Route("api/access")]
public sealed class AccessController : ControllerBase
{
    [Authorize]
    [HttpGet("authenticated")]
    public IActionResult Authenticated() => AccessGranted("AuthenticatedUser");

    [Authorize(Policy = Policies.AdminOnly)]
    [HttpGet("admin")]
    public IActionResult Admin() => AccessGranted(Policies.AdminOnly);

    [Authorize(Policy = Policies.Management)]
    [HttpGet("management")]
    public IActionResult Management() => AccessGranted(Policies.Management);

    [Authorize(Policy = Policies.TechnicalStaff)]
    [HttpGet("technical-staff")]
    public IActionResult TechnicalStaff() => AccessGranted(Policies.TechnicalStaff);

    [Authorize(Policy = Policies.StandardOnly)]
    [HttpGet("standard")]
    public IActionResult Standard() => AccessGranted(Policies.StandardOnly);

    private OkObjectResult AccessGranted(string policy) => Ok(new
    {
        message = "Acesso autorizado.",
        policy,
        userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
        role = User.FindFirstValue(ClaimTypes.Role)
    });
}
