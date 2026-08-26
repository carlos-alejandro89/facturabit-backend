using FacturaBit.Api.Contracts.Responses;
using FacturaBit.Api.Contracts.Requests;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Infrastructure.Security.Cryptography;
using FacturaBit.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Security.Claims;

namespace FacturaBit.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/emisores")]
public sealed class EmisoresController : ControllerBase
{
    private readonly ICertificateStorage _certificateStorage;
    private readonly OpenSSL _openSsl;
    private readonly ICertificadoSelloDigitalRepository _repository;
    private readonly IAesEncryptionService _encryptionService;
    private readonly ICertificadoDigitalService _certificateService;

    public EmisoresController(
        ICertificateStorage certificateStorage,
        OpenSSL openSsl,
        ICertificadoSelloDigitalRepository repository,
        IAesEncryptionService encryptionService,
        ICertificadoDigitalService certificateService)
    {
        _certificateStorage = certificateStorage;
        _openSsl = openSsl;
        _repository = repository;
        _encryptionService = encryptionService;
        _certificateService = certificateService;
    }

    [HttpGet("{rfc}/certificados")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<CertificadoDigitalDto>>>> ObtenerCertificados(
        string rfc,
        CancellationToken cancellationToken)
    {
        string? redComercialGuid = User.FindFirstValue("red_comercial_guid");
        if (string.IsNullOrWhiteSpace(redComercialGuid))
        {
            return Unauthorized(new ApiResponse<IReadOnlyCollection<CertificadoDigitalDto>>
            {
                Success = false,
                Mensaje = "La sesión no contiene una organización válida.",
                HttpCode = StatusCodes.Status401Unauthorized
            });
        }

        IReadOnlyCollection<CertificadoDigitalDto>? certificados =
            await _certificateService.ObtenerPorEmisorAsync(
                _certificateStorage.NormalizarRfc(rfc),
                redComercialGuid,
                cancellationToken);

        if (certificados is null)
        {
            return NotFound(new ApiResponse<IReadOnlyCollection<CertificadoDigitalDto>>
            {
                Success = false,
                Mensaje = "El emisor no existe o no pertenece a la organización actual.",
                HttpCode = StatusCodes.Status404NotFound
            });
        }

        return Ok(new ApiResponse<IReadOnlyCollection<CertificadoDigitalDto>>
        {
            Success = true,
            Mensaje = "Certificados digitales obtenidos correctamente.",
            HttpCode = StatusCodes.Status200OK,
            Data = certificados
        });
    }

    [HttpPost("{rfc}/certificado")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<CertificadoResponse>>> GuardarCertificado(
        string rfc,
        [FromForm] CargaCertificadoRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Certificado is null || request.Certificado.Length == 0)
        {
            return BadRequest(CrearRespuestaError("Debe proporcionar un archivo .cer válido."));
        }

        if (request.Llave is null || request.Llave.Length == 0)
        {
            return BadRequest(CrearRespuestaError("Debe proporcionar un archivo .key válido."));
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(CrearRespuestaError("Debe proporcionar la contraseña de la llave privada."));
        }

        if (!string.Equals(Path.GetExtension(request.Certificado.FileName), ".cer", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(CrearRespuestaError("El certificado debe tener extensión .cer."));
        }

        if (!string.Equals(Path.GetExtension(request.Llave.FileName), ".key", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(CrearRespuestaError("La llave privada debe tener extensión .key."));
        }

        try
        {
            string? redComercialGuid = User.FindFirstValue("red_comercial_guid");
            if (string.IsNullOrWhiteSpace(redComercialGuid))
            {
                return Unauthorized(CrearRespuestaError(
                    "La sesión no contiene una organización válida.",
                    StatusCodes.Status401Unauthorized));
            }

            string normalizedRfc = _certificateStorage.NormalizarRfc(rfc);
            EntidadFiscal? entidadFiscal = await _repository.ObtenerEntidadFiscalPorRfcYRedComercialAsync(
                normalizedRfc,
                redComercialGuid,
                cancellationToken);
            if (entidadFiscal is null)
            {
                return NotFound(CrearRespuestaError(
                    "El emisor no existe o no pertenece a la organización actual.",
                    StatusCodes.Status404NotFound));
            }

            string storedFileName = $"{normalizedRfc}.cer";
            string storedKeyFileName = $"{normalizedRfc}.key";
            await using MemoryStream certificateStream = new();
            await using MemoryStream keyStream = new();
            await request.Certificado.CopyToAsync(certificateStream, cancellationToken);
            await request.Llave.CopyToAsync(keyStream, cancellationToken);

            byte[] certificateData = certificateStream.ToArray();
            byte[] keyData = keyStream.ToArray();
            CertificateInfo certificateInfo = _openSsl.InfoCertificado(certificateData);
            DateTime currentDate = DateTime.Now;

            if (currentDate < certificateInfo.ValidoDesde)
            {
                return BadRequest(CrearRespuestaCertificado(
                    storedFileName,
                    storedKeyFileName,
                    certificateInfo,
                    false,
                    "El certificado aún no es válido; su periodo de vigencia no ha comenzado.",
                    StatusCodes.Status400BadRequest));
            }

            if (currentDate > certificateInfo.ValidoHasta)
            {
                return BadRequest(CrearRespuestaCertificado(
                    storedFileName,
                    storedKeyFileName,
                    certificateInfo,
                    false,
                    "El certificado no es válido; su periodo de vigencia ha finalizado.",
                    StatusCodes.Status400BadRequest));
            }

            if (!_openSsl.CertificadoYLlaveCorresponden(certificateData, keyData, request.Password))
            {
                return BadRequest(CrearRespuestaCertificado(
                    storedFileName,
                    storedKeyFileName,
                    certificateInfo,
                    false,
                    "La contraseña es incorrecta, la llave privada no es válida o no corresponde al certificado.",
                    StatusCodes.Status400BadRequest));
            }

            await _certificateStorage.GuardarCertificadoYLlaveAsync(
                normalizedRfc,
                certificateData,
                keyData,
                cancellationToken);

            CertificadoSelloDigital certificado = new()
            {
                EntidadFiscalId = entidadFiscal.Id,
                EntidadFiscal = entidadFiscal,
                ArchivoCer = storedFileName,
                ArchivoKey = storedKeyFileName,
                PasswordCert = _encryptionService.Cifrar(request.Password),
                ValidoDesde = certificateInfo.ValidoDesde.ToUniversalTime(),
                ValidoHasta = certificateInfo.ValidoHasta.ToUniversalTime(),
                TamanoClave = certificateInfo.TamanoClave ?? 0,
                NumeroSerie = certificateInfo.NumeroSerie,
                HuellaDigital = certificateInfo.HuellaDigital
            };
            await _repository.InsertarAsync(certificado, cancellationToken);

            return Ok(CrearRespuestaCertificado(
                storedFileName,
                storedKeyFileName,
                certificateInfo,
                true,
                "Certificado válido.",
                StatusCodes.Status200OK));
        }
        catch (CryptographicException)
        {
            return BadRequest(CrearRespuestaError("El archivo no contiene un certificado válido."));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(CrearRespuestaError(exception.Message));
        }
    }

    private static ApiResponse<CertificadoResponse> CrearRespuestaCertificado(
        string fileName,
        string keyFileName,
        CertificateInfo certificateInfo,
        bool certificadoValido,
        string mensaje,
        int httpCode)
    {
        CertificadoResponse data = new()
        {
            Archivo = fileName,
            ArchivoKey = keyFileName,
            CertificadoValido = certificadoValido,
            Sujeto = certificateInfo.Sujeto,
            Emisor = certificateInfo.Emisor,
            ValidoDesde = certificateInfo.ValidoDesde,
            ValidoHasta = certificateInfo.ValidoHasta,
            TamanoClave = certificateInfo.TamanoClave,
            NumeroSerie = certificateInfo.NumeroSerie,
            HuellaDigital = certificateInfo.HuellaDigital,
            Extensiones = certificateInfo.Extensiones
                .Select(extension => new CertificadoExtensionResponse
                {
                    Nombre = extension.Nombre,
                    Oid = extension.Oid,
                    EsCritica = extension.EsCritica,
                    Valor = extension.Valor
                })
                .ToArray()
        };

        return new ApiResponse<CertificadoResponse>
        {
            Success = certificadoValido,
            Mensaje = mensaje,
            HttpCode = httpCode,
            Data = data
        };
    }

    private static ApiResponse<CertificadoResponse> CrearRespuestaError(
        string mensaje,
        int httpCode = StatusCodes.Status400BadRequest)
    {
        return new ApiResponse<CertificadoResponse>
        {
            Success = false,
            Mensaje = mensaje,
            HttpCode = httpCode,
            Data = null
        };
    }
}
