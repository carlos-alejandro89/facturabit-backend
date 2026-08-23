using System.Text;
using System.Text.RegularExpressions;
using FacturaBit.Application.Abstractions;

namespace FacturaBit.Infrastructure.Persistence;

public sealed partial class CertificateStorage : ICertificateStorage
{
    private readonly string _rootPath;

    public CertificateStorage(IWebHostEnvironment environment)
    {
        _rootPath = Path.Combine(environment.ContentRootPath, "Certificados");
    }

    public string NormalizarRfc(string rfc)
    {
        string normalized = rfc.Trim().ToUpperInvariant();
        if (!RfcPattern().IsMatch(normalized))
        {
            throw new ArgumentException("El RFC proporcionado no tiene un formato válido.", nameof(rfc));
        }

        return normalized;
    }

    public string ObtenerRutaCertificado(string rfc)
    {
        string normalized = NormalizarRfc(rfc);
        return Path.Combine(_rootPath, normalized, $"{normalized}.cer");
    }

    public string ObtenerRutaLlave(string rfc)
    {
        string normalized = NormalizarRfc(rfc);
        return Path.Combine(_rootPath, normalized, $"{normalized}.key");
    }

    public bool ExisteCertificado(string rfc) => File.Exists(ObtenerRutaCertificado(rfc));
    public bool ExisteLlave(string rfc) => File.Exists(ObtenerRutaLlave(rfc));

    public async Task GuardarCertificadoAsync(string rfc, byte[] data, CancellationToken cancellationToken)
    {
        string path = ObtenerRutaCertificado(rfc);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await File.WriteAllBytesAsync(path, data, cancellationToken);
    }

    public async Task GuardarCertificadoYLlaveAsync(
        string rfc,
        byte[] certificateData,
        byte[] keyData,
        CancellationToken cancellationToken)
    {
        string certificatePath = ObtenerRutaCertificado(rfc);
        string keyPath = ObtenerRutaLlave(rfc);
        Directory.CreateDirectory(Path.GetDirectoryName(certificatePath)!);

        await File.WriteAllBytesAsync(certificatePath, certificateData, cancellationToken);
        await File.WriteAllBytesAsync(keyPath, keyData, cancellationToken);
    }

    public string ObtenerRutaRelativaCfdi(string rfc, string? serie, string folio, string uuid)
    {
        string normalized = NormalizarRfc(rfc);
        string safeFolio = SafeFilePart(folio);
        string uuidPrefix = ObtenerPrefijoUuid(uuid);
        string fileName = string.IsNullOrWhiteSpace(serie)
            ? $"CFDI_{safeFolio}_{uuidPrefix}.xml"
            : $"CFDI_{SafeFilePart(serie)}_{safeFolio}_{uuidPrefix}.xml";
        return Path.Combine("Certificados", normalized, fileName);
    }

    public async Task GuardarCfdiAsync(
        string rfc,
        string? serie,
        string folio,
        string uuid,
        string xml,
        CancellationToken cancellationToken)
    {
        string relativePath = ObtenerRutaRelativaCfdi(rfc, serie, folio, uuid);
        string path = Path.Combine(Path.GetDirectoryName(_rootPath)!, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await File.WriteAllTextAsync(path, xml, new UTF8Encoding(false), cancellationToken);
    }

    private static string ObtenerPrefijoUuid(string uuid)
    {
        string normalizedUuid = uuid.Trim();
        if (normalizedUuid.Length < 8)
        {
            throw new ArgumentException("El UUID del CFDI no tiene un formato válido.", nameof(uuid));
        }

        return SafeFilePart(normalizedUuid[..8]);
    }

    private static string SafeFilePart(string value)
    {
        char[] invalid = Path.GetInvalidFileNameChars();
        return new string(value.Where(character => !invalid.Contains(character)).ToArray());
    }

    [GeneratedRegex(@"^[A-ZÑ&]{3,4}\d{6}[A-Z0-9]{3}$", RegexOptions.CultureInvariant)]
    private static partial Regex RfcPattern();
}
