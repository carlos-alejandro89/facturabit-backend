using FacturaBit.Api.Contracts.Requests;
using FacturaBit.Api.Contracts.Responses;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FacturaBit.Api.Controllers;

[ApiController]
[Authorize]
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
        string? redComercialGuid = ObtenerRedComercialGuid();
        if (redComercialGuid is null)
        {
            return Unauthorized(Respuesta<IReadOnlyCollection<EntidadFiscalDto>>(
                false, "La sesión no contiene una organización válida.", StatusCodes.Status401Unauthorized, null));
        }

        IReadOnlyCollection<EntidadFiscalDto> entities = await _service.ObtenerPorRedComercialAsync(redComercialGuid, cancellationToken);
        return Ok(Respuesta(true, "Entidades fiscales obtenidas correctamente.", StatusCodes.Status200OK, entities));
    }

    [HttpGet("{guid}")]
    public async Task<ActionResult<ApiResponse<EntidadFiscalDto>>> ObtenerPorGuid(
        string guid,
        CancellationToken cancellationToken)
    {
        string? redComercialGuid = ObtenerRedComercialGuid();
        if (redComercialGuid is null)
        {
            return Unauthorized(Respuesta<EntidadFiscalDto>(
                false, "La sesión no contiene una organización válida.", StatusCodes.Status401Unauthorized, null));
        }

        EntidadFiscalDto? entity = await _service.ObtenerPorGuidAsync(guid, redComercialGuid, cancellationToken);
        return entity is null
            ? NotFound(Respuesta<EntidadFiscalDto>(false, "La entidad fiscal no existe.", StatusCodes.Status404NotFound, null))
            : Ok(Respuesta(true, "Entidad fiscal obtenida correctamente.", StatusCodes.Status200OK, entity));
    }

    [HttpPut("emisores")]
    public async Task<ActionResult<ApiResponse<EntidadFiscalDto>>> Upsert(
        UpsertEntidadFiscalRequest request,
        CancellationToken cancellationToken)
    {
        string? redComercialGuid = ObtenerRedComercialGuid();
        if (redComercialGuid is null)
        {
            return Unauthorized(Respuesta<EntidadFiscalDto>(
                false, "La sesión no contiene una organización válida.", StatusCodes.Status401Unauthorized, null));
        }

        try
        {
            bool isNew = string.IsNullOrWhiteSpace(request.Guid);
            EntidadFiscalDto entity = await _service.UpsertAsync(redComercialGuid, new UpsertEntidadFiscalDto
            {
                Guid = request.Guid,
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

            return Ok(Respuesta(
                true,
                isNew ? "Emisor creado correctamente." : "Emisor actualizado correctamente.",
                StatusCodes.Status200OK,
                entity));
        }
        catch (EntidadNoEncontradaException exception)
        {
            return NotFound(Respuesta<EntidadFiscalDto>(false, exception.Message, StatusCodes.Status404NotFound, null));
        }
        catch (ConflictoException exception)
        {
            return Conflict(Respuesta<EntidadFiscalDto>(false, exception.Message, StatusCodes.Status409Conflict, null));
        }
    }

    [HttpPost]
    [AllowAnonymous]
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
                Telefono = request.Telefono,
                SuscripcionGuid = request.SuscripcionGuid
            }, cancellationToken);

            ApiResponse<CrearEntidadFiscalDto> response = Respuesta(
                true,
                "Entidad fiscal, cuenta SuperAdministrador y orden de compra creadas correctamente.",
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
        string? redComercialGuid = ObtenerRedComercialGuid();
        if (redComercialGuid is null)
        {
            return Unauthorized(Respuesta<EntidadFiscalDto>(
                false, "La sesión no contiene una organización válida.", StatusCodes.Status401Unauthorized, null));
        }

        try
        {
            EntidadFiscalDto updated = await _service.UpsertAsync(redComercialGuid, new UpsertEntidadFiscalDto
            {
                Guid = guid,
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

            return Ok(Respuesta(true, "Entidad fiscal actualizada correctamente.", StatusCodes.Status200OK, updated));
        }
        catch (EntidadNoEncontradaException exception)
        {
            return NotFound(Respuesta<EntidadFiscalDto>(
                false, exception.Message, StatusCodes.Status404NotFound, null));
        }
        catch (ConflictoException exception)
        {
            return Conflict(Respuesta<EntidadFiscalDto>(
                false, exception.Message, StatusCodes.Status409Conflict, null));
        }
    }

    [HttpDelete("{guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> Eliminar(string guid, CancellationToken cancellationToken)
    {
        string? redComercialGuid = ObtenerRedComercialGuid();
        if (redComercialGuid is null)
        {
            return Unauthorized(Respuesta<bool>(
                false, "La sesión no contiene una organización válida.", StatusCodes.Status401Unauthorized, false));
        }

        bool deleted = await _service.EliminarAsync(guid, redComercialGuid, cancellationToken);
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

    private string? ObtenerRedComercialGuid() => User.FindFirstValue("red_comercial_guid");
}
