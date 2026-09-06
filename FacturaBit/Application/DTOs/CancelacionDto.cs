using System.ComponentModel.DataAnnotations;

namespace FacturaBit.Application.DTOs;

public sealed class CancelacionDto : IValidatableObject
{
    [Required, MaxLength(13)]
    public string RFC { get; init; } = string.Empty;

    [Required, MaxLength(36)]
    public string UUID { get; init; } = string.Empty;

    [Required, RegularExpression("0[1-4]", ErrorMessage = "El motivo de cancelación debe estar entre 01 y 04.")]
    public string Motivo { get; init; } = string.Empty;

    [MaxLength(36)]
    public string? FolioSustitucion { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.Equals(Motivo?.Trim(), "01", StringComparison.Ordinal))
        {
            yield break;
        }

        if (string.IsNullOrWhiteSpace(FolioSustitucion))
        {
            yield return new ValidationResult(
                "El UUID de sustitución es requerido cuando el motivo de cancelación es 01.",
                [nameof(FolioSustitucion)]);
            yield break;
        }

        if (!Guid.TryParse(FolioSustitucion.Trim(), out _))
        {
            yield return new ValidationResult(
                "El UUID de sustitución no tiene un formato válido.",
                [nameof(FolioSustitucion)]);
        }
    }
}

public sealed class ProveedorCancelacionRequestDto
{
    public required string Rfc { get; init; }
    public required string Uuid { get; init; }
    public required string Motivo { get; init; }
    public string? FolioSustitucion { get; init; }
    public required string RfcReceptor { get; init; }
    public decimal Total { get; init; }
    public required string CertificadoBase64 { get; init; }
    public required string LlaveBase64 { get; init; }
    public required string Password { get; init; }
}
