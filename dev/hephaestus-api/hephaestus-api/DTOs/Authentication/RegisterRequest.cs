using System.ComponentModel.DataAnnotations;

using Hephaestus.Api.Validation;

namespace Hephaestus.Api.DTOs.Authentication;

public sealed class RegisterRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public required string Name { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public required string Email { get; set; }

    [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
    [StrongPassword]
    public required string Password { get; set; }
}
