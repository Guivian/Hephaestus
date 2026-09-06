
using System.ComponentModel.DataAnnotations;

namespace Hephaestus.Api.DTOs.Authentication;

public sealed class GoogleLoginRequest
{
    [Required(ErrorMessage = "O ID token Google é obrigatório.")]
    public required string IdToken { get; set; }
}

public sealed class ExternalLoginExchangeRequest
{
    [Required(ErrorMessage = "O código de login externo é obrigatório.")]
    public required string Code { get; set; }
}
