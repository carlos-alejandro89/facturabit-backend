using FacturaBit.Api.Contracts.Responses;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacturaBit.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/sat/regimenes-fiscales")]
public sealed class SatRegimenFiscalController : ControllerBase
{
    private readonly ISatRegimenFiscalService _service;

    public SatRegimenFiscalController(ISatRegimenFiscalService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<SatRegimenFiscalDto>>>> ObtenerTodos(
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<SatRegimenFiscalDto> regimenes = await _service.ObtenerTodosAsync(cancellationToken);
        return Ok(new ApiResponse<IReadOnlyCollection<SatRegimenFiscalDto>>
        {
            Success = true,
            Mensaje = "Regímenes fiscales obtenidos correctamente.",
            HttpCode = StatusCodes.Status200OK,
            Data = regimenes
        });
    }
}
