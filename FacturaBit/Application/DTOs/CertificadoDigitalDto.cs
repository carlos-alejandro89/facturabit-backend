namespace FacturaBit.Application.DTOs;

public sealed class CertificadoDigitalDto
{
    public int Id { get; init; }
    public required string ArchivoCer { get; init; }
    public required string ArchivoKey { get; init; }
    public DateTime ValidoDesde { get; init; }
    public DateTime ValidoHasta { get; init; }
    public int TamanoClave { get; init; }
    public required string NumeroSerie { get; init; }
    public required string HuellaDigital { get; init; }
    public required string Estado { get; init; }
    public bool Vigente { get; init; }
}
