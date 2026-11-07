using System.ComponentModel.DataAnnotations;

namespace FacturaBit.Api.Contracts.Requests;

public sealed class RequestPasswordRecoveryRequest
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    [MaxLength(254)]
    public string Email { get; init; } = string.Empty;
}

public sealed class VerifyPasswordRecoveryCodeRequest
{
    [Required, EmailAddress, MaxLength(254)]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "El código de recuperación es obligatorio.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "El código debe contener exactamente seis dígitos.")]
    public string Code { get; init; } = string.Empty;
}

public sealed class ResetPasswordRequest : IValidatableObject
{
    [Required, EmailAddress, MaxLength(254)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "El código debe contener exactamente seis dígitos.")]
    public string Code { get; init; } = string.Empty;

    [Required(ErrorMessage = "La contraseña nueva es obligatoria.")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos ocho caracteres.")]
    [MaxLength(200)]
    public string NewPassword { get; init; } = string.Empty;

    [Required(ErrorMessage = "Confirma la contraseña nueva.")]
    [MaxLength(200)]
    public string ConfirmPassword { get; init; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.Equals(NewPassword, ConfirmPassword, StringComparison.Ordinal))
        {
            yield return new ValidationResult(
                "Las contraseñas no coinciden.",
                [nameof(ConfirmPassword)]);
        }
    }
}
