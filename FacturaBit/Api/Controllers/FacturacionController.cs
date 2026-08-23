using FacturaBit.Api.Contracts.Responses;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FacturaBit.Api.Controllers;

[ApiController]
[Route("api/facturacion")]
public sealed class FacturacionController : ControllerBase
{
    private readonly IFacturacionService _service;
    private readonly ILogger<FacturacionController> _logger;

    public FacturacionController(IFacturacionService service, ILogger<FacturacionController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost("emitir-cfdi")]
    public async Task<ActionResult<ApiResponse<FacturacionResultDto>>> EmitirCfdi(
        InvoiceModelDto invoice,
        [FromQuery] bool esGlobal = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
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
    public async Task<ActionResult<ApiResponse<CancelacionResultDto>>> CancelarCfdi(
        CancelacionDto cancelacion,
        CancellationToken cancellationToken)
    {
        try
        {
            CancelacionResultDto result = await _service.CancelarAsync(cancelacion, cancellationToken);
            return Ok(Respuesta(true, "CFDI cancelado correctamente.", StatusCodes.Status200OK, result));
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
