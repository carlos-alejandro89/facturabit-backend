using System.Security.Claims;
using FacturaBit.Api.Contracts.Responses;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacturaBit.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/comprobantes")]
public sealed class ComprobantesController : ControllerBase
{
    private readonly IComprobanteFiscalService _service;

    public ComprobantesController(IComprobanteFiscalService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<ComprobantesPageDto>>> Obtener(
        [FromQuery] string? emisorGuid,
        [FromQuery] string? busqueda,
        [FromQuery] string? estatus,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        string? redComercialGuid = User.FindFirstValue("red_comercial_guid");
        if (redComercialGuid is null)
            return Unauthorized(Respuesta<ComprobantesPageDto>(false, "La sesión no contiene una organización válida.", 401, null));

        ComprobantesPageDto result = await _service.ObtenerAsync(
            redComercialGuid, emisorGuid, busqueda, estatus, pagina, tamanoPagina, cancellationToken);

        return Ok(Respuesta(true, "Comprobantes obtenidos correctamente.", 200, result));
    }

    [HttpGet("{guid}/xml")]
    public async Task<ActionResult<ApiResponse<ComprobanteXmlDto>>> ObtenerXml(
        string guid,
        CancellationToken cancellationToken)
    {
        string? redComercialGuid = User.FindFirstValue("red_comercial_guid");
        if (redComercialGuid is null)
            return Unauthorized(Respuesta<ComprobanteXmlDto>(false, "La sesión no contiene una organización válida.", 401, null));

        ComprobanteXmlDto? result = await _service.ObtenerXmlAsync(guid, redComercialGuid, cancellationToken);
        return result is null
            ? NotFound(Respuesta<ComprobanteXmlDto>(false, "El XML del comprobante no está disponible.", 404, null))
            : Ok(Respuesta(true, "XML obtenido correctamente.", 200, result));
    }

    private static ApiResponse<T> Respuesta<T>(bool success, string mensaje, int httpCode, T? data) => new()
    {
        Success = success,
        Mensaje = mensaje,
        HttpCode = httpCode,
        Data = data
    };
}
