using System.Text;
using System.Xml.Linq;
using System.Text.Json;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Application.Exceptions;
using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Features;

public sealed class FacturacionService : IFacturacionService
{
    private const int EstatusSolicitudTimbradoId = 1;
    private const string MensajeTimbradoExitoso = "CFDI timbrado correctamente.";
    private readonly ICertificateStorage _certificateStorage;
    private readonly ICfdiXmlBuilder _xmlBuilder;
    private readonly ICfdiSigner _signer;
    private readonly ITimbradoService _timbradoService;
    private readonly ICertificadoSelloDigitalRepository _certificateRepository;
    private readonly IAesEncryptionService _encryptionService;
    private readonly ILogger<FacturacionService> _logger;
    private readonly IEntidadFiscalRepository _entidadFiscalRepository;
    private readonly IComprobanteFiscalRepository _comprobanteRepository;

    public FacturacionService(
        ICertificateStorage certificateStorage,
        ICfdiXmlBuilder xmlBuilder,
        ICfdiSigner signer,
        ITimbradoService timbradoService,
        ICertificadoSelloDigitalRepository certificateRepository,
        IAesEncryptionService encryptionService,
        ILogger<FacturacionService> logger,
        IEntidadFiscalRepository entidadFiscalRepository,
        IComprobanteFiscalRepository comprobanteRepository)
    {
        _certificateStorage = certificateStorage;
        _xmlBuilder = xmlBuilder;
        _signer = signer;
        _timbradoService = timbradoService;
        _certificateRepository = certificateRepository;
        _encryptionService = encryptionService;
        _logger = logger;
        _entidadFiscalRepository = entidadFiscalRepository;
        _comprobanteRepository = comprobanteRepository;
    }

    public async Task<FacturacionResultDto> EmitirAsync(
        InvoiceModelDto invoice,
        bool esGlobal,
        CancellationToken cancellationToken)
    {
        string rfc = _certificateStorage.NormalizarRfc(invoice.RfcEmisor);
        EntidadFiscal? entidadFiscal = await _certificateRepository.ObtenerEntidadFiscalPorRfcAsync(
            rfc,
            cancellationToken);
        if (entidadFiscal is null)
        {
            throw new FacturacionException("No existe una entidad fiscal registrada con el RFC emisor.");
        }

        DateTime utcNow = DateTime.UtcNow;
        ComprobanteFiscal comprobante = new()
        {
            EstatusCfdiId = EstatusSolicitudTimbradoId,
            EstatusCfdi = null!,
            EntidadFiscalId = entidadFiscal.Id,
            EntidadFiscal = entidadFiscal,
            Folio = invoice.FolioInterno,
            SerieCfdi = invoice.Serie,
            ReceptorRfc = invoice.RfcReceptor,
            ReceptorNombre = invoice.Receptor,
            JsonRequest = JsonSerializer.SerializeToDocument(new { esGlobal, invoice }),
            TipoRequest = "TIMBRADO",
            Success = false,
            Mensaje = "Solicitud de timbrado recibida.",
            NoCertificadoSat = string.Empty,
            NoCertificadoEmisor = string.Empty,
            FolioFiscal = string.Empty,
            SelloSat = string.Empty,
            SelloEmisor = string.Empty,
            FechaTimbrado = DateTime.UnixEpoch,
            CadenaOriginalSat = string.Empty,
            Guid = Guid.NewGuid().ToString(),
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };
        try
        {
            await _comprobanteRepository.InsertarAsync(comprobante, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "No se pudo insertar la solicitud de timbrado para el RFC {Rfc}.",
                rfc);
            string detail = exception.InnerException?.Message ?? exception.Message;
            throw new FacturacionException(
                $"No fue posible registrar la solicitud de timbrado: {detail}",
                exception);
        }

        try
        {
            FacturacionResultDto result = await ProcesarEmisionAsync(
                invoice,
                esGlobal,
                rfc,
                entidadFiscal,
                cancellationToken);
            string relativeXmlPath = _certificateStorage.ObtenerRutaRelativaCfdi(
                rfc,
                invoice.Serie,
                invoice.FolioInterno,
                result.Uuid);
            await _comprobanteRepository.MarcarVigenteAsync(
                comprobante.Id,
                result,
                MensajeTimbradoExitoso,
                relativeXmlPath,
                cancellationToken);
            return result;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            string responseMessage = exception is FacturacionException or ArgumentException
                ? exception.Message
                : "Ocurrió un error inesperado al emitir el CFDI.";

            try
            {
                await _comprobanteRepository.MarcarErrorAsync(
                    comprobante.Id,
                    responseMessage,
                    CancellationToken.None);
            }
            catch (Exception traceException)
            {
                _logger.LogError(
                    traceException,
                    "No se pudo marcar como ERROR el comprobante fiscal {ComprobanteId}.",
                    comprobante.Id);
            }

            throw;
        }
    }

    private async Task<FacturacionResultDto> ProcesarEmisionAsync(
        InvoiceModelDto invoice,
        bool esGlobal,
        string rfc,
        EntidadFiscal entidadFiscal,
        CancellationToken cancellationToken)
    {
        ValidarInformacionGlobal(invoice, esGlobal);
        ValidarDisponibilidadTimbres(entidadFiscal);

        if (!_certificateStorage.ExisteCertificado(rfc))
        {
            throw new FacturacionException($"No se encontró el certificado en Certificados/{rfc}/{rfc}.cer.");
        }

        if (!_certificateStorage.ExisteLlave(rfc))
        {
            throw new FacturacionException($"No se encontró la llave privada en Certificados/{rfc}/{rfc}.key.");
        }

        string certificatePath = _certificateStorage.ObtenerRutaCertificado(rfc);
        string keyPath = _certificateStorage.ObtenerRutaLlave(rfc);
        Domain.Entities.CertificadoSelloDigital? storedCertificate =
            await _certificateRepository.ObtenerVigentePorRfcAsync(rfc, cancellationToken);
        if (storedCertificate is null)
        {
            throw new FacturacionException("No existe un certificado de sello digital vigente registrado para el emisor.");
        }

        string certificatePassword;
        try
        {
            certificatePassword = _encryptionService.Descifrar(storedCertificate.PasswordCert);
        }
        catch (Exception exception) when (exception is FormatException or System.Security.Cryptography.CryptographicException)
        {
            _logger.LogError(exception, "Error al descifrar la contraseña del CSD para el RFC {Rfc}.", rfc);
            throw new FacturacionException(
                $"Error al descifrar la contraseña del certificado: {exception.Message}",
                exception);
        }

        CertificateSigningDataDto certificate = EjecutarEtapa(
            () => _signer.ObtenerDatosCertificado(certificatePath),
            "lectura del certificado",
            rfc);
        DateTime fechaEmision = invoice.Fecha == default ? DateTime.Now : invoice.Fecha;

        string unsignedXml = EjecutarEtapa(
            () => _xmlBuilder.Construir(
                invoice,
                fechaEmision,
                string.Empty,
                esGlobal,
                certificate.CertificadoBase64,
                certificate.NumeroCertificado),
            "construcción del XML sin sello",
            rfc);

        string sello = EjecutarEtapa(
            () => _signer.Firmar(unsignedXml, keyPath, certificatePassword),
            "firma del XML",
            rfc);

        string signedXml = EjecutarEtapa(
            () => _xmlBuilder.Construir(
                invoice,
                fechaEmision,
                sello,
                esGlobal,
                certificate.CertificadoBase64,
                certificate.NumeroCertificado),
            "construcción del XML firmado",
            rfc);

        TimbradoResultDto stamped = await EjecutarEtapaAsync(
            () => _timbradoService.TimbrarAsync(signedXml, cancellationToken),
            "timbrado del XML",
            rfc);
        if (!stamped.Success)
        {
            string providerMessage = stamped.Mensaje ?? "El proveedor rechazó el timbrado del CFDI.";
            _logger.LogWarning("El proveedor rechazó el timbrado para el RFC {Rfc}: {Message}", rfc, providerMessage);
            throw new FacturacionException($"Error durante el timbrado del XML: {providerMessage}");
        }

        await DescontarTimbreAsync(rfc, cancellationToken);

        if (string.IsNullOrWhiteSpace(stamped.Cfdi))
        {
            throw new FacturacionException(
                "El proveedor confirmó el timbrado, pero no devolvió el XML del CFDI.");
        }

        string formattedCfdi = EjecutarEtapa(
            () => FormatearXml(stamped.Cfdi),
            "formateo del XML timbrado",
            rfc);

        await EjecutarEtapaAsync(
            async () =>
            {
                await _certificateStorage.GuardarCfdiAsync(
                    rfc,
                    invoice.Serie,
                    invoice.FolioInterno,
                    stamped.Uuid ?? string.Empty,
                    formattedCfdi,
                    cancellationToken);
                return true;
            },
            "almacenamiento del XML timbrado",
            rfc);

        return new FacturacionResultDto
        {
            NoCertificadoSat = stamped.NoCertificadoSat ?? string.Empty,
            NoCertificadoEmisor = stamped.NoCertificadoEmisor ?? string.Empty,
            Uuid = stamped.Uuid ?? string.Empty,
            SelloSat = stamped.SelloSat ?? string.Empty,
            SelloEmisor = stamped.SelloEmisor ?? string.Empty,
            FechaTimbrado = stamped.FechaTimbrado ?? string.Empty,
            CadenaOriginalSat = stamped.CadenaOriginalSat ?? string.Empty,
            CfdiXmlBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(formattedCfdi))
        };
    }

    private static string FormatearXml(string xml)
    {
        XDocument document = XDocument.Parse(xml);
        return document.ToString();
    }

    private T EjecutarEtapa<T>(Func<T> action, string stage, string rfc)
    {
        try
        {
            return action();
        }
        catch (FacturacionException exception)
        {
            _logger.LogError(exception, "Error durante {Stage} para el RFC {Rfc}.", stage, rfc);
            throw new FacturacionException($"Error durante {stage}: {exception.Message}", exception);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error inesperado durante {Stage} para el RFC {Rfc}.", stage, rfc);
            throw new FacturacionException($"Error durante {stage}: {exception.Message}", exception);
        }
    }

    private async Task<T> EjecutarEtapaAsync<T>(Func<Task<T>> action, string stage, string rfc)
    {
        try
        {
            return await action();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (FacturacionException exception)
        {
            _logger.LogError(exception, "Error durante {Stage} para el RFC {Rfc}.", stage, rfc);
            throw new FacturacionException($"Error durante {stage}: {exception.Message}", exception);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error inesperado durante {Stage} para el RFC {Rfc}.", stage, rfc);
            throw new FacturacionException($"Error durante {stage}: {exception.Message}", exception);
        }
    }

    private static void ValidarInformacionGlobal(InvoiceModelDto invoice, bool esGlobal)
    {
        if (!esGlobal)
        {
            return;
        }

        List<string> missingFields = [];
        if (string.IsNullOrWhiteSpace(invoice.Periodicidad)) missingFields.Add(nameof(invoice.Periodicidad));
        if (string.IsNullOrWhiteSpace(invoice.Meses)) missingFields.Add(nameof(invoice.Meses));
        if (string.IsNullOrWhiteSpace(invoice.YYYY)) missingFields.Add(nameof(invoice.YYYY));

        if (missingFields.Count > 0)
        {
            throw new FacturacionException(
                $"Para una factura global son obligatorios los campos: {string.Join(", ", missingFields)}.");
        }
    }

    private async Task DescontarTimbreAsync(string rfc, CancellationToken cancellationToken)
    {
        EntidadFiscal? entidadFiscal = await _entidadFiscalRepository.ObtenerPorRfcAsync(rfc, cancellationToken);
        if (entidadFiscal is null)
        {
            throw new FacturacionException($"No existe una entidad fiscal registrada con el RFC {rfc}.");
        }

        if (!entidadFiscal.EsPrepago)
        {
            return;
        }

        bool updated = await EjecutarEtapaAsync(
            () => _entidadFiscalRepository.DescontarTimbrePorRfcAsync(rfc, cancellationToken),
            "actualización de timbres disponibles",
            rfc);

        if (!updated)
        {
            _logger.LogError(
                "No fue posible descontar un timbre prepago para el RFC {Rfc}; el saldo pudo agotarse por otra solicitud.",
                rfc);
            throw new FacturacionException(
                "La operación fue aceptada por el proveedor, pero ya no hay timbres disponibles para descontar.");
        }
    }

    private static void ValidarDisponibilidadTimbres(EntidadFiscal entidadFiscal)
    {
        if (entidadFiscal.EsPrepago && entidadFiscal.TimbresDisponibles <= 0)
        {
            throw new FacturacionException("No hay timbres disponibles para realizar la operación.");
        }
    }

    public async Task<CancelacionResultDto> CancelarAsync(
        CancelacionDto cancelacion,
        CancellationToken cancellationToken)
    {
        string rfc = _certificateStorage.NormalizarRfc(cancelacion.RFC);
        EntidadFiscal? entidadFiscal = await _entidadFiscalRepository.ObtenerPorRfcAsync(rfc, cancellationToken);
        if (entidadFiscal is null)
        {
            throw new FacturacionException("No existe una entidad fiscal registrada con el RFC proporcionado.");
        }

        ValidarDisponibilidadTimbres(entidadFiscal);
        ProveedorCancelacionResultDto result = await _timbradoService.CancelarAsync(cancelacion, cancellationToken);
        if (!result.Success)
        {
            string message = result.Mensaje ?? "No fue posible cancelar el CFDI.";
            if (!string.IsNullOrWhiteSpace(result.Detalle))
            {
                message = $"{message} {result.Detalle}";
            }
            throw new FacturacionException(message);
        }

        await DescontarTimbreAsync(rfc, cancellationToken);

        if (string.IsNullOrWhiteSpace(result.Acuse))
        {
            throw new FacturacionException(
                "El proveedor confirmó la cancelación, pero no devolvió el acuse correspondiente.");
        }

        return new CancelacionResultDto
        {
            AcuseBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(result.Acuse))
        };
    }
}
