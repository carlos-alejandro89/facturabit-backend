namespace FacturaBit.Application.Abstractions;

public interface ICfdiSigner
{
    CertificateSigningDataDto ObtenerDatosCertificado(string certificatePath);
    string Firmar(string xml, string keyPath, string password);
}

public sealed record CertificateSigningDataDto(string CertificadoBase64, string NumeroCertificado);
