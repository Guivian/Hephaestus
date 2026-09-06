using System.ComponentModel.DataAnnotations;

namespace Hephaestus.Api.DTOs.Authentication;

public sealed class LoginRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
}