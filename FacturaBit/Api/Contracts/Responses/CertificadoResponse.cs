namespace FacturaBit.Api.Contracts.Responses;

public sealed class CertificadoResponse
{
    public required string Archivo { get; init; }
    public required string ArchivoKey { get; init; }
    public bool CertificadoValido { get; init; }
    public required string Sujeto { get; init; }
    public required string Emisor { get; init; }
    public DateTime ValidoDesde { get; init; }
    public DateTime ValidoHasta { get; init; }
    public int? TamanoClave { get; init; }
    public required string NumeroSerie { get; init; }
    public required string HuellaDigital { get; init; }
    public required IReadOnlyCollection<CertificadoExtensionResponse> Extensiones { get; init; }
}

public sealed class CertificadoExtensionResponse
{
    public string? Nombre { get; init; }
    public string? Oid { get; init; }
    public bool EsCritica { get; init; }
    public required string Valor { get; init; }
}
