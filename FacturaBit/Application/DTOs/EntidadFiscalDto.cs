namespace FacturaBit.Application.DTOs;

public sealed class EntidadFiscalDto
{
    public required string Guid { get; init; }
    public int? SatRegimenFiscalId { get; init; }
    public string? SatRegimenFiscalClave { get; init; }
    public string? SatRegimenFiscalNombre { get; init; }
    public required string RazonSocial { get; init; }
    public required string Rfc { get; init; }
    public string? RegistroPatronal { get; init; }
    public string? Calle { get; init; }
    public string? NumExt { get; init; }
    public string? NumInt { get; init; }
    public string? Colonia { get; init; }
    public string? Ciudad { get; init; }
    public string? Estado { get; init; }
    public required string CodigoPostal { get; init; }
    public int TimbresDisponibles { get; init; }
    public bool EsPrepago { get; init; }
    public bool CertificadoVigente { get; init; }
    public string? NombreComercial { get; init; }
    public string? TelefonoComercial { get; init; }
    public string? TelefonoComercial2 { get; init; }
    public string? CorreoComercial { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public sealed class CrearEntidadFiscalDto
{
    public required EntidadFiscalDto EntidadFiscal { get; init; }
    public required string PasswordTemporal { get; init; }
    public DateTime PasswordExpiresAt { get; init; }
    public required string OrdenCompraGuid { get; init; }
    public required string FolioOrdenCompra { get; init; }
}

public sealed class CrearEntidadFiscalCommandDto
{
    public required string NombreRedComercial { get; init; }
    public required string FullName { get; init; }
    public required string Email { get; init; }
    public required string Telefono { get; init; }
    public required string SuscripcionGuid { get; init; }
}

public sealed class ActualizarEntidadFiscalDto
{
    public bool? EsPrepago { get; init; }
    public int? SatRegimenFiscalId { get; init; }
    public required string RazonSocial { get; init; }
    public required string Rfc { get; init; }
    public string? RegistroPatronal { get; init; }
    public string? Calle { get; init; }
    public string? NumExt { get; init; }
    public string? NumInt { get; init; }
    public string? Colonia { get; init; }
    public string? Ciudad { get; init; }
    public string? Estado { get; init; }
    public required string CodigoPostal { get; init; }
    public string? NombreComercial { get; init; }
    public string? TelefonoComercial { get; init; }
    public string? TelefonoComercial2 { get; init; }
    public string? CorreoComercial { get; init; }
}

public sealed class UpsertEntidadFiscalDto
{
    public string? Guid { get; init; }
    public bool? EsPrepago { get; init; }
    public int? SatRegimenFiscalId { get; init; }
    public required string RazonSocial { get; init; }
    public required string Rfc { get; init; }
    public string? RegistroPatronal { get; init; }
    public string? Calle { get; init; }
    public string? NumExt { get; init; }
    public string? NumInt { get; init; }
    public string? Colonia { get; init; }
    public string? Ciudad { get; init; }
    public string? Estado { get; init; }
    public required string CodigoPostal { get; init; }
    public string? NombreComercial { get; init; }
    public string? TelefonoComercial { get; init; }
    public string? TelefonoComercial2 { get; init; }
    public string? CorreoComercial { get; init; }
}
