using FacturaBit.Api.Contracts.Requests;
using FacturaBit.Api.Contracts.Responses;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FacturaBit.Api.Controllers;

[ApiController]
[Route("api/entidades-fiscales")]
public sealed class EntidadesFiscalesController : ControllerBase
{
    private readonly IEntidadFiscalService _service;

    public EntidadesFiscalesController(IEntidadFiscalService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<EntidadFiscalDto>>>> ObtenerTodos(
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<EntidadFiscalDto> entities = await _service.ObtenerTodosAsync(cancellationToken);
        return Ok(Respuesta(true, "Entidades fiscales obtenidas correctamente.", StatusCodes.Status200OK, entities));
    }

    [HttpGet("{guid}")]
    public async Task<ActionResult<ApiResponse<EntidadFiscalDto>>> ObtenerPorGuid(
        string guid,
        CancellationToken cancellationToken)
    {
        EntidadFiscalDto? entity = await _service.ObtenerPorGuidAsync(guid, cancellationToken);
        return entity is null
            ? NotFound(Respuesta<EntidadFiscalDto>(false, "La entidad fiscal no existe.", StatusCodes.Status404NotFound, null))
            : Ok(Respuesta(true, "Entidad fiscal obtenida correctamente.", StatusCodes.Status200OK, entity));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CrearEntidadFiscalDto>>> Crear(
        CrearEntidadFiscalRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            CrearEntidadFiscalDto created = await _service.CrearAsync(new CrearEntidadFiscalCommandDto
            {
                NombreRedComercial = request.NombreRedComercial,
                FullName = request.FullName,
                Email = request.Email,
                Telefono = request.Telefono
            }, cancellationToken);

            ApiResponse<CrearEntidadFiscalDto> response = Respuesta(
                true,
                "Entidad fiscal y cuenta SuperAdministrador creadas correctamente.",
                StatusCodes.Status201Created,
                created);

            return CreatedAtAction(nameof(ObtenerPorGuid), new { guid = created.EntidadFiscal.Guid }, response);
        }
        catch (EntidadNoEncontradaException exception)
        {
            return NotFound(Respuesta<CrearEntidadFiscalDto>(
                false, exception.Message, StatusCodes.Status404NotFound, null));
        }
        catch (ConflictoException exception)
        {
            return Conflict(Respuesta<CrearEntidadFiscalDto>(
                false, exception.Message, StatusCodes.Status409Conflict, null));
        }
    }

    [HttpPut("{guid}")]
    public async Task<ActionResult<ApiResponse<EntidadFiscalDto>>> Actualizar(
        string guid,
        ActualizarEntidadFiscalRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            EntidadFiscalDto? updated = await _service.ActualizarAsync(guid, new ActualizarEntidadFiscalDto
            {
                EsPrepago = request.EsPrepago,
                SatRegimenFiscalId = request.SatRegimenFiscalId,
                RazonSocial = request.RazonSocial,
                Rfc = request.Rfc,
                RegistroPatronal = request.RegistroPatronal,
                Calle = request.Calle,
                NumExt = request.NumExt,
                NumInt = request.NumInt,
                Colonia = request.Colonia,
                Ciudad = request.Ciudad,
                Estado = request.Estado,
                CodigoPostal = request.CodigoPostal,
                NombreComercial = request.NombreComercial,
                TelefonoComercial = request.TelefonoComercial,
                TelefonoComercial2 = request.TelefonoComercial2,
                CorreoComercial = request.CorreoComercial
            }, cancellationToken);

            return updated is null
                ? NotFound(Respuesta<EntidadFiscalDto>(false, "La entidad fiscal no existe.", StatusCodes.Status404NotFound, null))
                : Ok(Respuesta(true, "Entidad fiscal actualizada correctamente.", StatusCodes.Status200OK, updated));
        }
        catch (EntidadNoEncontradaException exception)
        {
            return NotFound(Respuesta<EntidadFiscalDto>(
                false, exception.Message, StatusCodes.Status404NotFound, null));
        }
    }

    [HttpDelete("{guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Eliminar(string guid, CancellationToken cancellationToken)
    {
        bool deleted = await _service.EliminarAsync(guid, cancellationToken);
        return deleted
            ? Ok(Respuesta(true, "Entidad fiscal eliminada correctamente.", StatusCodes.Status200OK, true))
            : NotFound(Respuesta<bool>(false, "La entidad fiscal no existe.", StatusCodes.Status404NotFound, false));
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
