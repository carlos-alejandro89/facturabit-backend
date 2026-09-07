namespace FacturaBit.Application.DTOs;

public sealed class SatMotivoCancelacionDto
{
    public int Id { get; init; }
    public required string Guid { get; init; }
    public required string Clave { get; init; }
    public required string Nombre { get; init; }
    public bool RequiereFolioSustitucion { get; init; }
}
