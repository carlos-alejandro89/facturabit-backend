namespace FacturaBit.Infrastructure.Security.Cryptography;

public sealed record CertificateInfo(
    string Sujeto,
    string Emisor,
    DateTime ValidoDesde,
    DateTime ValidoHasta,
    int? TamanoClave,
    string NumeroSerie,
    string HuellaDigital,
    IReadOnlyCollection<CertificateExtensionInfo> Extensiones);

public sealed record CertificateExtensionInfo(
    string? Nombre,
    string? Oid,
    bool EsCritica,
    string Valor);
