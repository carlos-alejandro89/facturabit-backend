using System.Security.Claims;
using FacturaBit.Api.Contracts.Requests;
using FacturaBit.Api.Contracts.Responses;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacturaBit.Api.Controllers;

[ApiController]
[Authorize(Roles = "SUPERADMIN,ADMIN")]
[Route("api/integraciones")]
public sealed class ApiClientsController : ControllerBase
{
    private readonly IApiClientService _service;
    public ApiClientsController(IApiClientService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ApiClientDto>>>> Obtener(CancellationToken cancellationToken)
    {
        string? redGuid = User.FindFirstValue("red_comercial_guid");
        if (redGuid is null) return Unauthorized(CreateResponse<IReadOnlyCollection<ApiClientDto>>(false, "La sesión no contiene una organización válida.", 401, null));
        var clients = await _service.ObtenerAsync(redGuid, cancellationToken);
        return Ok(CreateResponse(true, "Integraciones obtenidas correctamente.", 200, clients));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ApiClientCreatedDto>>> Crear(CreateApiClientRequest request, CancellationToken cancellationToken)
    {
        string? redGuid = User.FindFirstValue("red_comercial_guid");
        if (redGuid is null) return Unauthorized(CreateResponse<ApiClientCreatedDto>(false, "La sesión no contiene una organización válida.", 401, null));
        try
        {
            ApiClientCreatedDto created = await _service.CrearAsync(redGuid, request.Nombre, request.Scopes, cancellationToken);
            return StatusCode(201, CreateResponse(true, "Credencial creada. Guarda el client_secret; no volverá a mostrarse.", 201, created));
        }
        catch (ConflictoException exception)
        {
            return Conflict(CreateResponse<ApiClientCreatedDto>(false, exception.Message, 409, null));
        }
        catch (EntidadNoEncontradaException exception)
        {
            return NotFound(CreateResponse<ApiClientCreatedDto>(false, exception.Message, 404, null));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(CreateResponse<ApiClientCreatedDto>(false, exception.Message, 400, null));
        }
    }

    [HttpDelete("{guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Revocar(string guid, CancellationToken cancellationToken)
    {
        string? redGuid = User.FindFirstValue("red_comercial_guid");
        if (redGuid is null) return Unauthorized(CreateResponse(false, "La sesión no contiene una organización válida.", 401, false));
        bool revoked = await _service.RevocarAsync(redGuid, guid, cancellationToken);
        return revoked
            ? Ok(CreateResponse(true, "Credencial revocada correctamente.", 200, true))
            : NotFound(CreateResponse(false, "La credencial no existe.", 404, false));
    }

    private static ApiResponse<T> CreateResponse<T>(bool success, string message, int code, T? data) => new()
    {
        Success = success, Mensaje = message, HttpCode = code, Data = data
    };
}
