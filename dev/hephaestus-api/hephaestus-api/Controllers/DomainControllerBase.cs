using System.Security.Claims;
using Hephaestus.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hephaestus.Api.Controllers;

public abstract class DomainControllerBase : ControllerBase
{
    protected int CurrentUserId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
        ? id : throw new DomainException(StatusCodes.Status401Unauthorized, "Token sem identificador de utilizador.");
    protected string CurrentRole => User.FindFirstValue(ClaimTypes.Role)
        ?? throw new DomainException(StatusCodes.Status401Unauthorized, "Token sem role.");
}
