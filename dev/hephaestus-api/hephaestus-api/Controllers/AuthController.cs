using Hephaestus.Api.DTOs.Authentication;
using Hephaestus.Api.Services;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.WebUtilities;

using Hephaestus.Api.Security;

namespace Hephaestus.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    Hephaestus.Api.Services.AuthenticationService authenticationService,
    TokenService tokenService,
    TwoFactorService twoFactorService,
    GoogleAuthService googleAuthService,
    IConfiguration configuration) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await authenticationService.RegisterAsync(request.Name, request.Email, request.Password, cancellationToken);

            return Created("", new
            {
                userId = user.UsersId,
                user.Name,
                user.Email,
                role = user.Role.Name
            });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = exception.Message
            });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await authenticationService.ValidateCredentialsAsync(request.Email, request.Password, cancellationToken);

        if (user is null)
        {
            return Unauthorized(new
            {
                message = "E-mail ou palavra-passe inválidos."
            });
        }

        if (user.Role.Name == RoleNames.Technician || user.Is2FAEnabled)
        {
            var challengeId = await twoFactorService.CreateChallengeAsync(user, cancellationToken);
            return Accepted(new
            {
                requiresTwoFactor = true,
                challengeId,
                expiresInSeconds = 300
            });
        }

        var response = await tokenService.CreateSessionAsync(user, cancellationToken);
        return Ok(response);
    }

    [HttpPost("2fa/verify")]
    public async Task<IActionResult> VerifyTwoFactor(
        TwoFactorRequest request,
        CancellationToken cancellationToken)
    {
        if (request.ChallengeId == Guid.Empty)
            return BadRequest(new { message = "O identificador do desafio é obrigatório." });

        var user = await twoFactorService.VerifyAsync(
            request.ChallengeId, request.Code, cancellationToken);

        if (user is null)
            return Unauthorized(new { message = "Código inválido, expirado ou sem tentativas disponíveis." });

        var response = await tokenService.CreateSessionAsync(user, cancellationToken);
        return Ok(response);
    }

    [HttpPost("2fa/resend")]
    public async Task<IActionResult> ResendTwoFactor(
        TwoFactorResendRequest request,
        CancellationToken cancellationToken)
    {
        if (request.ChallengeId == Guid.Empty)
            return BadRequest(new { message = "O identificador do desafio é obrigatório." });

        try
        {
            var challengeId = await twoFactorService.ResendAsync(
                request.ChallengeId, cancellationToken);

            return Accepted(new
            {
                requiresTwoFactor = true,
                challengeId,
                expiresInSeconds = 300
            });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpGet("google/web/login")]
    public IActionResult GoogleWebLogin()
    {
        var completeUrl = Url.ActionLink(
            nameof(GoogleWebComplete),
            values: null,
            protocol: Request.Scheme)
            ?? throw new InvalidOperationException("Não foi possível criar o callback Google.");

        return Challenge(
            new AuthenticationProperties { RedirectUri = completeUrl },
            GoogleAuthService.GoogleScheme);
    }

    [HttpGet("google/web/complete")]
    public async Task<IActionResult> GoogleWebComplete(CancellationToken cancellationToken)
    {
        var authentication = await HttpContext.AuthenticateAsync(
            GoogleAuthService.ExternalCookieScheme);

        if (!authentication.Succeeded || authentication.Principal is null)
            return Unauthorized(new { message = "Não foi possível validar a conta Google." });

        var googleId = authentication.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = authentication.Principal.FindFirstValue(ClaimTypes.Email);
        var name = authentication.Principal.FindFirstValue(ClaimTypes.Name) ?? email;

        await HttpContext.SignOutAsync(GoogleAuthService.ExternalCookieScheme);

        if (string.IsNullOrWhiteSpace(googleId) || string.IsNullOrWhiteSpace(email))
            return Unauthorized(new { message = "A conta Google não forneceu os dados necessários." });

        try
        {
            var user = await googleAuthService.AuthenticateWebAsync(
                googleId, email, name ?? email, cancellationToken);
            var code = await googleAuthService.CreateExternalCodeAsync(user, cancellationToken);
            var callbackUrl = configuration["Clients:WebCallbackUrl"]
                ?? throw new InvalidOperationException("Clients:WebCallbackUrl não está configurado.");

            return Redirect(QueryHelpers.AddQueryString(callbackUrl, "code", code));
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("google/web/exchange")]
    public async Task<IActionResult> ExchangeGoogleWebCode(
        ExternalLoginExchangeRequest request,
        CancellationToken cancellationToken)
    {
        var user = await googleAuthService.ExchangeExternalCodeAsync(request.Code, cancellationToken);
        if (user is null)
            return Unauthorized(new { message = "Código de login externo inválido ou expirado." });

        return Ok(await tokenService.CreateSessionAsync(user, cancellationToken));
    }

    [HttpPost("google/mobile")]
    public async Task<IActionResult> GoogleMobileLogin(
        GoogleLoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await googleAuthService.AuthenticateMobileAsync(
                request.IdToken, cancellationToken);

            if (user is null)
                return Unauthorized(new { message = "ID token Google inválido." });

            return Ok(await tokenService.CreateSessionAsync(user, cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var response = await tokenService.RefreshSessionAsync(request.RefreshToken, cancellationToken);

        return response is null
            ? Unauthorized(new { message = "Refresh token inválido ou expirado." })
            : Ok(response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        await tokenService.RevokeSessionAsync(request.RefreshToken, cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            name = User.FindFirstValue(ClaimTypes.Name),
            email = User.FindFirstValue(ClaimTypes.Email),
            role = User.FindFirstValue(ClaimTypes.Role)
        });
    }
}
