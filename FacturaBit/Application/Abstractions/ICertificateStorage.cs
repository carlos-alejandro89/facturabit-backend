namespace FacturaBit.Application.Abstractions;

public interface ICertificateStorage
{
    string NormalizarRfc(string rfc);
    string ObtenerRutaCertificado(string rfc);
    string ObtenerRutaLlave(string rfc);
    bool ExisteCertificado(string rfc);
    bool ExisteLlave(string rfc);
    Task<string> LeerCertificadoBase64Async(string rfc, CancellationToken cancellationToken);
    Task<string> LeerLlaveBase64Async(string rfc, CancellationToken cancellationToken);
    Task GuardarCertificadoAsync(string rfc, byte[] data, CancellationToken cancellationToken);
    Task GuardarCertificadoYLlaveAsync(
        string rfc,
        byte[] certificateData,
        byte[] keyData,
        CancellationToken cancellationToken);
    string ObtenerRutaRelativaCfdi(string rfc, string? serie, string folio, string uuid);
    Task GuardarCfdiAsync(
        string rfc,
        string? serie,
        string folio,
        string uuid,
        string xml,
        CancellationToken cancellationToken);
    Task<string?> LeerCfdiAsync(string relativePath, CancellationToken cancellationToken);
    Task<string> GuardarAcuseCancelacionAsync(
        string rfc,
        string? serie,
        string folio,
        string uuid,
        string acuse,
        byte[] pdf,
        CancellationToken cancellationToken);
    Task<byte[]?> LeerPdfAsync(string relativeXmlPath, CancellationToken cancellationToken);
}
