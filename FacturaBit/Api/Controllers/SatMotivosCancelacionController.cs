using FacturaBit.Api.Contracts.Responses;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FacturaBit.Api.Controllers;

[ApiController]
[Authorize]
[EnableRateLimiting("ApiClients")]
[Route("api/sat/motivos-cancelacion")]
public sealed class SatMotivosCancelacionController : ControllerBase
{
    private readonly ISatMotivosCancelacionService _service;

    public SatMotivosCancelacionController(ISatMotivosCancelacionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<SatMotivoCancelacionDto>>>>
        ObtenerTodos(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<SatMotivoCancelacionDto> motivos =
            await _service.ObtenerTodosAsync(cancellationToken);

        return Ok(new ApiResponse<IReadOnlyCollection<SatMotivoCancelacionDto>>
        {
            Success = true,
            Mensaje = "Motivos de cancelación obtenidos correctamente.",
            HttpCode = StatusCodes.Status200OK,
            Data = motivos
        });
    }
}
