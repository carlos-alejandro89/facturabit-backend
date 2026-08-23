namespace FacturaBit.Application.Abstractions;

public interface ICertificateStorage
{
    string NormalizarRfc(string rfc);
    string ObtenerRutaCertificado(string rfc);
    string ObtenerRutaLlave(string rfc);
    bool ExisteCertificado(string rfc);
    bool ExisteLlave(string rfc);
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
}
