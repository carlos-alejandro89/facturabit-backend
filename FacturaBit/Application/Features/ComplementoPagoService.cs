using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Application.Exceptions;
using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Features;

public sealed class ComplementoPagoService : IComplementoPagoService
{
    private const int EstatusSolicitudTimbradoId = 1;
    private const string MensajeTimbradoExitoso = "Complemento de pago timbrado correctamente.";

    private readonly ICertificateStorage _certificateStorage;
    private readonly IComplementoPagoXmlBuilder _xmlBuilder;
    private readonly ICfdiSigner _signer;
    private readonly ITimbradoService _timbradoService;
    private readonly ICertificadoSelloDigitalRepository _certificateRepository;
    private readonly IAesEncryptionService _encryptionService;
    private readonly IEntidadFiscalRepository _entidadFiscalRepository;
    private readonly IComprobanteFiscalRepository _comprobanteRepository;
    private readonly ILogger<ComplementoPagoService> _logger;

    public ComplementoPagoService(
        ICertificateStorage certificateStorage,
        IComplementoPagoXmlBuilder xmlBuilder,
        ICfdiSigner signer,
        ITimbradoService timbradoService,
        ICertificadoSelloDigitalRepository certificateRepository,
        IAesEncryptionService encryptionService,
        IEntidadFiscalRepository entidadFiscalRepository,
        IComprobanteFiscalRepository comprobanteRepository,
        ILogger<ComplementoPagoService> logger)
    {
        _certificateStorage = certificateStorage;
        _xmlBuilder = xmlBuilder;
        _signer = signer;
        _timbradoService = timbradoService;
        _certificateRepository = certificateRepository;
        _encryptionService = encryptionService;
        _entidadFiscalRepository = entidadFiscalRepository;
        _comprobanteRepository = comprobanteRepository;
        _logger = logger;
    }

    public async Task<FacturacionResultDto> EmitirAsync(
        ComplementoPagoDto complemento,
        CancellationToken cancellationToken)
    {
        if (complemento is null)
            throw new FacturacionException("La información del complemento de pago es obligatoria.");
        if (string.IsNullOrWhiteSpace(complemento.RfcEmisor))
            throw new FacturacionException("El RFC del emisor es obligatorio.");

        string rfc = _certificateStorage.NormalizarRfc(complemento.RfcEmisor);
        EntidadFiscal? entidadFiscal = await _certificateRepository.ObtenerEntidadFiscalPorRfcAsync(
            rfc,
            cancellationToken);
        if (entidadFiscal is null)
            throw new FacturacionException("No existe una entidad fiscal registrada con el RFC emisor.");

        DateTime utcNow = DateTime.UtcNow;
        ComprobanteFiscal comprobante = new()
        {
            EstatusCfdiId = EstatusSolicitudTimbradoId,
            EstatusCfdi = null!,
            EntidadFiscalId = entidadFiscal.Id,
            EntidadFiscal = entidadFiscal,
            Folio = complemento.FolioInterno,
            SerieCfdi = complemento.Serie,
            ReceptorRfc = complemento.RfcReceptor,
            ReceptorNombre = complemento.Receptor,
            JsonRequest = JsonSerializer.SerializeToDocument(complemento),
            TipoRequest = "COMPLEMENTO_PAGO",
            Success = false,
            Mensaje = "Solicitud de timbrado de complemento de pago recibida.",
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
                "No se pudo registrar la solicitud de complemento de pago para el RFC {Rfc}.",
                rfc);
            string detail = exception.InnerException?.Message ?? exception.Message;
            throw new FacturacionException(
                $"No fue posible registrar la solicitud de complemento de pago: {detail}",
                exception);
        }

        try
        {
            FacturacionResultDto result = await ProcesarEmisionAsync(
                complemento,
                rfc,
                entidadFiscal,
                cancellationToken);
            string relativeXmlPath = _certificateStorage.ObtenerRutaRelativaCfdi(
                rfc,
                complemento.Serie,
                complemento.FolioInterno,
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
                : "Ocurrió un error inesperado al emitir el complemento de pago.";

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
                    "No se pudo marcar como ERROR el complemento de pago {ComprobanteId}.",
                    comprobante.Id);
            }

            throw;
        }
    }

    private async Task<FacturacionResultDto> ProcesarEmisionAsync(
        ComplementoPagoDto complemento,
        string rfc,
        EntidadFiscal entidadFiscal,
        CancellationToken cancellationToken)
    {
        ValidarDisponibilidadTimbres(entidadFiscal);

        if (!_certificateStorage.ExisteCertificado(rfc))
            throw new FacturacionException($"No se encontró el certificado en Certificados/{rfc}/{rfc}.cer.");
        if (!_certificateStorage.ExisteLlave(rfc))
            throw new FacturacionException($"No se encontró la llave privada en Certificados/{rfc}/{rfc}.key.");

        string certificatePath = _certificateStorage.ObtenerRutaCertificado(rfc);
        string keyPath = _certificateStorage.ObtenerRutaLlave(rfc);
        CertificadoSelloDigital? storedCertificate =
            await _certificateRepository.ObtenerVigentePorRfcAsync(rfc, cancellationToken);
        if (storedCertificate is null)
            throw new FacturacionException("No existe un certificado de sello digital vigente registrado para el emisor.");

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
        DateTime fechaEmision = complemento.Fecha == default ? DateTime.Now : complemento.Fecha;

        string unsignedXml = EjecutarEtapa(
            () => _xmlBuilder.Construir(
                complemento,
                fechaEmision,
                string.Empty,
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
                complemento,
                fechaEmision,
                sello,
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
            string providerMessage = stamped.Mensaje ?? "El proveedor rechazó el complemento de pago.";
            _logger.LogWarning(
                "El proveedor rechazó el complemento de pago para el RFC {Rfc}: {Message}",
                rfc,
                providerMessage);
            throw new FacturacionException($"Error durante el timbrado del XML: {providerMessage}");
        }

        if (string.IsNullOrWhiteSpace(stamped.Cfdi))
            throw new FacturacionException("El proveedor confirmó el timbrado, pero no devolvió el XML del complemento de pago.");

        await DescontarTimbreAsync(rfc, cancellationToken);
        string formattedCfdi = EjecutarEtapa(
            () => XDocument.Parse(stamped.Cfdi).ToString(),
            "formateo del XML timbrado",
            rfc);
        await EjecutarEtapaAsync(
            async () =>
            {
                await _certificateStorage.GuardarCfdiAsync(
                    rfc,
                    complemento.Serie,
                    complemento.FolioInterno,
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

    private async Task DescontarTimbreAsync(string rfc, CancellationToken cancellationToken)
    {
        EntidadFiscal? entidadFiscal = await _entidadFiscalRepository.ObtenerPorRfcAsync(rfc, cancellationToken);
        if (entidadFiscal is null)
            throw new FacturacionException($"No existe una entidad fiscal registrada con el RFC {rfc}.");
        if (!entidadFiscal.EsPrepago)
            return;

        bool updated = await EjecutarEtapaAsync(
            () => _entidadFiscalRepository.DescontarTimbrePorRfcAsync(rfc, cancellationToken),
            "actualización de timbres disponibles",
            rfc);
        if (!updated)
            throw new FacturacionException("La operación fue aceptada por el proveedor, pero ya no hay timbres disponibles para descontar.");
    }

    private static void ValidarDisponibilidadTimbres(EntidadFiscal entidadFiscal)
    {
        if (entidadFiscal.EsPrepago && entidadFiscal.TimbresDisponibles <= 0)
            throw new FacturacionException("No hay timbres disponibles para realizar la operación.");
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
}
