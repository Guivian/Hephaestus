
using System.ComponentModel.DataAnnotations;

namespace Hephaestus.Api.DTOs.Authentication;

public sealed class TwoFactorRequest
{
    public Guid ChallengeId { get; set; }

    [Required(ErrorMessage = "O código de autenticação é obrigatório.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "O código deve conter exatamente seis números.")]
    public required string Code { get; set; }
}

public sealed class TwoFactorResendRequest
{
    public Guid ChallengeId { get; set; }
}
