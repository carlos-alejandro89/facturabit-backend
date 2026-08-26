using System.ComponentModel.DataAnnotations;

namespace FacturaBit.Api.Contracts.Requests;

public sealed class UpsertEntidadFiscalRequest
{
    public string? Guid { get; init; }
    public bool? EsPrepago { get; init; }
    public int? SatRegimenFiscalId { get; init; }

    [Required, MaxLength(300)]
    public string RazonSocial { get; init; } = string.Empty;

    [Required, MaxLength(13)]
    public string Rfc { get; init; } = string.Empty;

    public string? RegistroPatronal { get; init; }
    public string? Calle { get; init; }
    public string? NumExt { get; init; }
    public string? NumInt { get; init; }
    public string? Colonia { get; init; }
    public string? Ciudad { get; init; }
    public string? Estado { get; init; }

    [Required, RegularExpression(@"^\d{5}$")]
    public string CodigoPostal { get; init; } = string.Empty;

    public string? NombreComercial { get; init; }
    public string? TelefonoComercial { get; init; }
    public string? TelefonoComercial2 { get; init; }

    [EmailAddress]
    public string? CorreoComercial { get; init; }
}
