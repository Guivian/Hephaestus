
using System.ComponentModel.DataAnnotations;

namespace Hephaestus.Api.DTOs.Authentication;

public sealed class RefreshTokenRequest
{
    [Required(ErrorMessage = "O refresh token é obrigatório.")]
    public required string RefreshToken { get; set; }
}
