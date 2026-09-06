using System.Text;
using System.Text.Json;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Application.Exceptions;
using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Features;

public sealed class CancelacionCfdiService : ICancelacionCfdiService
{
    private readonly ICertificateStorage _certificateStorage;
    private readonly ICertificadoSelloDigitalRepository _certificateRepository;
    private readonly IAesEncryptionService _encryptionService;
    private readonly ITimbradoService _timbradoService;
    private readonly IEntidadFiscalRepository _entidadFiscalRepository;
    private readonly IComprobanteFiscalRepository _comprobanteRepository;
    private readonly ILogger<CancelacionCfdiService> _logger;

    public CancelacionCfdiService(
        ICertificateStorage certificateStorage,
        ICertificadoSelloDigitalRepository certificateRepository,
        IAesEncryptionService encryptionService,
        ITimbradoService timbradoService,
        IEntidadFiscalRepository entidadFiscalRepository,
        IComprobanteFiscalRepository comprobanteRepository,
        ILogger<CancelacionCfdiService> logger)
    {
        _certificateStorage = certificateStorage;
        _certificateRepository = certificateRepository;
        _encryptionService = encryptionService;
        _timbradoService = timbradoService;
        _entidadFiscalRepository = entidadFiscalRepository;
        _comprobanteRepository = comprobanteRepository;
        _logger = logger;
    }

    public async Task<CancelacionResultDto> CancelarAsync(
        CancelacionDto cancelacion,
        string redComercialGuid,
        CancellationToken cancellationToken)
    {
        string rfc = _certificateStorage.NormalizarRfc(cancelacion.RFC);
        string uuid = NormalizarUuid(cancelacion.UUID);
        string motivo = cancelacion.Motivo.Trim();
        string? folioSustitucion = ValidarYNormalizarFolioSustitucion(
            motivo,
            cancelacion.FolioSustitucion,
            uuid);

        ComprobanteFiscal? comprobante = await _comprobanteRepository.ObtenerPorUuidRfcYRedComercialAsync(
            uuid,
            rfc,
            redComercialGuid,
            cancellationToken);
        if (comprobante is null)
        {
            throw new FacturacionException(
                "No existe un CFDI con el UUID y RFC indicados dentro de la organización autenticada.");
        }

        if (comprobante.EstatusCfdiId == 3)
        {
            throw new FacturacionException("El CFDI ya se encuentra cancelado.");
        }

        EntidadFiscal entidadFiscal = comprobante.EntidadFiscal;
        ValidarDisponibilidadTimbres(entidadFiscal);

        CertificadoSelloDigital? certificate =
            await _certificateRepository.ObtenerVigentePorRfcAsync(rfc, cancellationToken);
        if (certificate is null)
        {
            throw new FacturacionException(
                "No existe un certificado de sello digital vigente registrado para el emisor.");
        }

        if (!_certificateStorage.ExisteCertificado(rfc) || !_certificateStorage.ExisteLlave(rfc))
        {
            throw new FacturacionException(
                "No se encontraron los archivos .cer y .key configurados para el emisor.");
        }

        string password;
        try
        {
            password = _encryptionService.Descifrar(certificate.PasswordCert);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "No fue posible descifrar la contraseña del CSD para cancelar {Uuid}.", uuid);
            throw new FacturacionException(
                $"No fue posible obtener la contraseña del certificado: {exception.Message}",
                exception);
        }

        ProveedorCancelacionRequestDto providerRequest = new()
        {
            Rfc = rfc,
            Uuid = uuid,
            Motivo = motivo,
            FolioSustitucion = folioSustitucion,
            RfcReceptor = _certificateStorage.NormalizarRfc(comprobante.ReceptorRfc),
            Total = ObtenerTotal(comprobante.JsonRequest),
            CertificadoBase64 = await _certificateStorage.LeerCertificadoBase64Async(rfc, cancellationToken),
            LlaveBase64 = await _certificateStorage.LeerLlaveBase64Async(rfc, cancellationToken),
            Password = password
        };

        await _comprobanteRepository.MarcarCancelacionEnProcesoAsync(comprobante.Id, cancellationToken);

        try
        {
            ProveedorCancelacionResultDto result = await _timbradoService.CancelarAsync(
                providerRequest,
                cancellationToken);
            if (!result.Success)
            {
                string message = ConstruirMensajeProveedor(result);
                await _comprobanteRepository.MarcarCancelacionErrorAsync(
                    comprobante.Id,
                    message,
                    CancellationToken.None);
                throw new FacturacionException(message);
            }

            bool cancelado = string.Equals(
                result.StatusSat,
                "Cancelado",
                StringComparison.OrdinalIgnoreCase);

            if (cancelado && string.IsNullOrWhiteSpace(result.Acuse))
            {
                const string message =
                    "El proveedor confirmó la cancelación, pero no devolvió el acuse correspondiente.";
                await _comprobanteRepository.MarcarCancelacionErrorAsync(
                    comprobante.Id,
                    message,
                    CancellationToken.None);
                throw new FacturacionException(message);
            }

            if (cancelado)
            {
                await DescontarTimbreAsync(entidadFiscal, cancellationToken);
                string path = await _certificateStorage.GuardarAcuseCancelacionAsync(
                    rfc,
                    uuid,
                    result.Acuse!,
                    cancellationToken);
                await _comprobanteRepository.MarcarCanceladoAsync(
                    comprobante.Id,
                    "CFDI cancelado correctamente.",
                    path,
                    cancellationToken);
            }

            return new CancelacionResultDto
            {
                AcuseBase64 = string.IsNullOrWhiteSpace(result.Acuse)
                    ? null
                    : Convert.ToBase64String(Encoding.UTF8.GetBytes(result.Acuse)),
                Cancelado = cancelado,
                EstatusSat = result.StatusSat,
                CodigoEstatusSat = result.StatusCodeSat,
                EsCancelable = result.IsCancelable,
                EstatusCancelacion = result.StatusCancelation,
                EstatusPorUuid = result.UuidStatuses
            };
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (FacturacionException exception)
        {
            try
            {
                await _comprobanteRepository.MarcarCancelacionErrorAsync(
                    comprobante.Id,
                    exception.Message,
                    CancellationToken.None);
            }
            catch (Exception persistenceException)
            {
                _logger.LogError(
                    persistenceException,
                    "No fue posible registrar el error de cancelación del CFDI {Uuid}.",
                    uuid);
            }

            throw;
        }
        catch (Exception exception)
        {
            string message = $"Error durante la cancelación del CFDI: {exception.Message}";
            _logger.LogError(exception, "Error al cancelar el CFDI {Uuid} del emisor {Rfc}.", uuid, rfc);
            try
            {
                await _comprobanteRepository.MarcarCancelacionErrorAsync(
                    comprobante.Id,
                    message,
                    CancellationToken.None);
            }
            catch (Exception persistenceException)
            {
                _logger.LogError(
                    persistenceException,
                    "No fue posible registrar el error de cancelación del CFDI {Uuid}.",
                    uuid);
            }

            throw new FacturacionException(message, exception);
        }
    }

    private async Task DescontarTimbreAsync(
        EntidadFiscal entidadFiscal,
        CancellationToken cancellationToken)
    {
        if (!entidadFiscal.EsPrepago)
        {
            return;
        }

        bool updated = await _entidadFiscalRepository.DescontarTimbrePorRfcAsync(
            entidadFiscal.Rfc,
            cancellationToken);
        if (!updated)
        {
            throw new FacturacionException(
                "La cancelación fue aceptada por el proveedor, pero no fue posible descontar el timbre.");
        }
    }

    private static void ValidarDisponibilidadTimbres(EntidadFiscal entidadFiscal)
    {
        if (entidadFiscal.EsPrepago && entidadFiscal.TimbresDisponibles <= 0)
        {
            throw new FacturacionException("No hay timbres disponibles para realizar la cancelación.");
        }
    }

    private static string NormalizarUuid(string uuid)
    {
        if (!Guid.TryParse(uuid.Trim(), out Guid parsed))
        {
            throw new FacturacionException("El UUID proporcionado no tiene un formato válido.");
        }

        return parsed.ToString("D").ToUpperInvariant();
    }

    private static decimal ObtenerTotal(JsonDocument request)
    {
        if (!request.RootElement.TryGetProperty("invoice", out JsonElement invoice) ||
            (!invoice.TryGetProperty("Total", out JsonElement total) &&
             !invoice.TryGetProperty("total", out total)) ||
            !total.TryGetDecimal(out decimal value) ||
            value <= 0)
        {
            throw new FacturacionException(
                "No fue posible obtener un total válido del CFDI almacenado.");
        }

        return value;
    }

    private static string? ValidarYNormalizarFolioSustitucion(
        string motivo,
        string? folioSustitucion,
        string uuidCancelado)
    {
        if (!string.Equals(motivo, "01", StringComparison.Ordinal))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(folioSustitucion))
        {
            throw new FacturacionException(
                "El UUID de sustitución es requerido cuando el motivo de cancelación es 01.");
        }

        string uuidSustitucion = NormalizarUuid(folioSustitucion);
        if (string.Equals(uuidSustitucion, uuidCancelado, StringComparison.OrdinalIgnoreCase))
        {
            throw new FacturacionException(
                "El UUID de sustitución debe ser diferente al UUID del CFDI que se desea cancelar.");
        }

        return uuidSustitucion;
    }

    private static string ConstruirMensajeProveedor(ProveedorCancelacionResultDto result)
    {
        string message = result.Mensaje ?? "El proveedor rechazó la cancelación del CFDI.";
        return string.IsNullOrWhiteSpace(result.Detalle)
            ? message
            : $"{message} Detalle: {result.Detalle}";
    }
}
