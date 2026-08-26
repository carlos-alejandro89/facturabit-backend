namespace FacturaBit.Application.DTOs;

public sealed class SatRegimenFiscalDto
{
    public int Id { get; init; }
    public required string Guid { get; init; }
    public required string Clave { get; init; }
    public required string Nombre { get; init; }
}
