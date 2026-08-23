using System.ComponentModel.DataAnnotations;

namespace FacturaBit.Api.Contracts.Requests;

public sealed class CrearEntidadFiscalRequest
{
    [Required, MaxLength(200)]
    public string NombreRedComercial { get; init; } = string.Empty;

    [Required, MaxLength(200)]
    public string FullName { get; init; } = string.Empty;

    [Required, EmailAddress, MaxLength(254)]
    public string Email { get; init; } = string.Empty;

    [Required, MaxLength(30)]
    public string Telefono { get; init; } = string.Empty;
}
