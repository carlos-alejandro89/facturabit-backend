using System.Security.Claims;
using FacturaBit.Api.Contracts.Responses;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace FacturaBit.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/facturacion")]
public sealed class FacturacionController : ControllerBase
{
    private readonly IFacturacionService _service;
    private readonly ICancelacionCfdiService _cancelacionService;
    private readonly IConsultaEstatusCfdiService _consultaEstatusService;
    private readonly IApiClientService _apiClientService;
    private readonly ILogger<FacturacionController> _logger;

    public FacturacionController(
        IFacturacionService service,
        ICancelacionCfdiService cancelacionService,
        IConsultaEstatusCfdiService consultaEstatusService,
        IApiClientService apiClientService,
        ILogger<FacturacionController> logger)
    {
        _service = service;
        _cancelacionService = cancelacionService;
        _consultaEstatusService = consultaEstatusService;
        _apiClientService = apiClientService;
        _logger = logger;
    }

    [HttpGet("estatus-sat")]
    [Authorize(Policy = "ConsultarEstatusCfdi")]
    [EnableRateLimiting("ApiClients")]
    public async Task<ActionResult<ApiResponse<ConsultaEstatusCfdiResultDto>>> ConsultarEstatusSat(
        [FromQuery] string rfc,
        [FromQuery] string uuid,
        CancellationToken cancellationToken)
    {
        try
        {
            string? redComercialGuid = User.FindFirstValue("red_comercial_guid");
            if (string.IsNullOrWhiteSpace(redComercialGuid))
            {
                return Unauthorized(Respuesta<ConsultaEstatusCfdiResultDto>(
                    false,
                    "La sesión no contiene una organización válida.",
                    StatusCodes.Status401Unauthorized,
                    null));
            }

            ConsultaEstatusCfdiResultDto result = await _consultaEstatusService.ConsultarAsync(
                rfc,
                uuid,
                redComercialGuid,
                cancellationToken);
            return Ok(Respuesta(
                true,
                "Estatus del CFDI consultado correctamente.",
                StatusCodes.Status200OK,
                result));
        }
        catch (FacturacionException exception)
        {
            return BadRequest(Respuesta<ConsultaEstatusCfdiResultDto>(
                false,
                exception.Message,
                StatusCodes.Status400BadRequest,
                null));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error inesperado al consultar el estatus SAT del CFDI {Uuid}.", uuid);
            return StatusCode(StatusCodes.Status500InternalServerError, Respuesta<ConsultaEstatusCfdiResultDto>(
                false,
                "Ocurrió un error inesperado al consultar el estatus del CFDI.",
                StatusCodes.Status500InternalServerError,
                null));
        }
    }

    [HttpPost("emitir-cfdi")]
    [Authorize(Policy = "EmitirCfdi")]
    [EnableRateLimiting("ApiClients")]
    public async Task<ActionResult<ApiResponse<FacturacionResultDto>>> EmitirCfdi(
        InvoiceModelDto invoice,
        [FromQuery] bool esGlobal = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.Equals(User.FindFirstValue("auth_type"), "client_credentials", StringComparison.Ordinal))
            {
                string redComercialId = User.FindFirstValue("red_comercial_id") ?? string.Empty;
                if (!await _apiClientService.PuedeEmitirAsync(redComercialId, invoice.RfcEmisor, cancellationToken))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, Respuesta<FacturacionResultDto>(false,
                        "El emisor no pertenece a la organización asociada con estas credenciales.",
                        StatusCodes.Status403Forbidden, null));
                }
            }

            FacturacionResultDto result = await _service.EmitirAsync(invoice, esGlobal, cancellationToken);
            return Ok(Respuesta(true, "CFDI timbrado correctamente.", StatusCodes.Status200OK, result));
        }
        catch (FacturacionException exception)
        {
            return BadRequest(Respuesta<FacturacionResultDto>(
                false, exception.Message, StatusCodes.Status400BadRequest, null));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(Respuesta<FacturacionResultDto>(
                false, exception.Message, StatusCodes.Status400BadRequest, null));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error inesperado al emitir el CFDI.");
            return StatusCode(StatusCodes.Status500InternalServerError, Respuesta<FacturacionResultDto>(
                false,
                "Ocurrió un error inesperado al emitir el CFDI.",
                StatusCodes.Status500InternalServerError,
                null));
        }
    }

    [HttpPost("cancelar-cfdi")]

    [EnableRateLimiting("ApiClients")]
    public async Task<ActionResult<ApiResponse<CancelacionResultDto>>> CancelarCfdi(
        CancelacionDto cancelacion,
        CancellationToken cancellationToken)
    {
        try
        {
            string? redComercialGuid = User.FindFirstValue("red_comercial_guid");
            if (string.IsNullOrWhiteSpace(redComercialGuid))
            {
                return Unauthorized(Respuesta<CancelacionResultDto>(
                    false,
                    "La sesión no contiene una organización válida.",
                    StatusCodes.Status401Unauthorized,
                    null));
            }

            CancelacionResultDto result = await _cancelacionService.CancelarAsync(
                cancelacion,
                redComercialGuid,
                cancellationToken);
            string message = result.Cancelado
                ? "CFDI cancelado correctamente."
                : "La solicitud fue procesada; la cancelación aún no ha sido confirmada por el SAT.";
            return Ok(Respuesta(true, message, StatusCodes.Status200OK, result));
        }
        catch (FacturacionException exception)
        {
            return BadRequest(Respuesta<CancelacionResultDto>(
                false, exception.Message, StatusCodes.Status400BadRequest, null));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error inesperado al cancelar el CFDI.");
            return StatusCode(StatusCodes.Status500InternalServerError, Respuesta<CancelacionResultDto>(
                false,
                "Ocurrió un error inesperado al cancelar el CFDI.",
                StatusCodes.Status500InternalServerError,
                null));
        }
    }

    private static ApiResponse<T> Respuesta<T>(bool success, string mensaje, int httpCode, T? data)
    {
        return new ApiResponse<T>
        {
            Success = success,
            Mensaje = mensaje,
            HttpCode = httpCode,
            Data = data
        };
    }
}
