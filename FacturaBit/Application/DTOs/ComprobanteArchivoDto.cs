namespace FacturaBit.Application.DTOs;

public sealed class ComprobanteArchivoDto
{
    public required string NombreArchivo { get; init; }
    public required byte[] Contenido { get; init; }
    public required string ContentType { get; init; }
}
