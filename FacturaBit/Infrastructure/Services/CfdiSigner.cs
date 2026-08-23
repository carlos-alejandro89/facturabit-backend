using System.Net;
using System.Xml;
using System.Xml.Xsl;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.Exceptions;
using FacturaBit.Infrastructure.Security.Cryptography;

namespace FacturaBit.Infrastructure.Services;

public sealed class CfdiSigner : ICfdiSigner
{
    private readonly string _xsltPath;
    private readonly OpenSSL _openSsl;

    public CfdiSigner(IWebHostEnvironment environment, OpenSSL openSsl)
    {
        _xsltPath = Path.Combine(
            environment.ContentRootPath,
            "Infrastructure",
            "Resources",
            "Cfdi",
            "cadenaoriginal_4_0.xslt");
        _openSsl = openSsl;
    }

    public string Firmar(string xml, string keyPath, string password)
    {
        if (!File.Exists(_xsltPath))
        {
            throw new FacturacionException("No se encontró la plantilla para generar la cadena original del CFDI.");
        }

        if (!File.Exists(keyPath))
        {
            throw new FacturacionException("No se encontró la llave privada .key del emisor.");
        }

        XslCompiledTransform transform = new();
        XmlReaderSettings readerSettings = new()
        {
            XmlResolver = new XmlUrlResolver { Credentials = CredentialCache.DefaultCredentials },
            DtdProcessing = DtdProcessing.Parse
        };
        XsltSettings xsltSettings = new(enableDocumentFunction: true, enableScript: false);

        try
        {
            using XmlReader xsltReader = XmlReader.Create(_xsltPath, readerSettings);
            transform.Load(xsltReader, xsltSettings, new XmlUrlResolver());
        }
        catch (XsltException exception)
        {
            string source = string.IsNullOrWhiteSpace(exception.SourceUri)
                ? _xsltPath
                : exception.SourceUri;
            string detail = ObtenerDetalleExcepcion(exception);
            throw new FacturacionException(
                $"No fue posible compilar el XSLT de la cadena original. Recurso: '{source}', " +
                $"línea {exception.LineNumber}, posición {exception.LinePosition}. Detalle: {detail}",
                exception);
        }

        using StringReader xmlText = new(xml);
        using XmlReader xmlReader = XmlReader.Create(xmlText);
        using StringWriter result = new();
        transform.Transform(xmlReader, null, result);

        string sello = _openSsl.SignString(keyPath, password, result.ToString());
        if (string.IsNullOrWhiteSpace(sello) || sello.StartsWith("Archivo .key no existe", StringComparison.Ordinal))
        {
            throw new FacturacionException("No fue posible firmar el CFDI. Verifique la llave privada y su contraseña.");
        }

        return sello;
    }

    private static string ObtenerDetalleExcepcion(Exception exception)
    {
        Exception current = exception;
        while (current.InnerException is not null)
        {
            current = current.InnerException;
        }

        return current.Message;
    }

    public CertificateSigningDataDto ObtenerDatosCertificado(string certificatePath)
    {
        if (!File.Exists(certificatePath))
        {
            throw new FacturacionException("No se encontró el certificado .cer del emisor.");
        }

        _openSsl.CertificateData(certificatePath, out string certificate, out string certificateNumber);
        return new CertificateSigningDataDto(certificate, certificateNumber);
    }
}
