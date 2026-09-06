using System.ComponentModel.DataAnnotations;

namespace Hephaestus.Api.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class StrongPasswordAttribute : ValidationAttribute
{
    public const int MinimumLength = 12;
    public const int MaximumLength = 128;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null) return ValidationResult.Success;
        if (value is not string password)
            return new ValidationResult("A palavra-passe tem um formato inválido.");

        var missingRequirements = new List<string>();

        if (password.Length < MinimumLength)
            missingRequirements.Add($"ter pelo menos {MinimumLength} caracteres");
        if (password.Length > MaximumLength)
            missingRequirements.Add($"ter no máximo {MaximumLength} caracteres");
        if (!password.Any(char.IsUpper))
            missingRequirements.Add("incluir pelo menos uma letra maiúscula");
        if (!password.Any(char.IsLower))
            missingRequirements.Add("incluir pelo menos uma letra minúscula");
        if (!password.Any(char.IsDigit))
            missingRequirements.Add("incluir pelo menos um número");
        if (!password.Any(character =>
                !char.IsLetterOrDigit(character) && !char.IsWhiteSpace(character)))
            missingRequirements.Add("incluir pelo menos um carácter especial");
        if (password.Any(char.IsWhiteSpace))
            missingRequirements.Add("não conter espaços");

        return missingRequirements.Count == 0 ? ValidationResult.Success : new ValidationResult($"Para ser forte, a palavra-passe deve {string.Join(", ", missingRequirements)}.");
    }
}
