using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Application.Exceptions;
using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Features;

public sealed class EntidadFiscalService : IEntidadFiscalService
{
    private const string SuperAdministradorRole = "SUPERADMIN";
    private readonly IEntidadFiscalRepository _repository;
    private readonly IPasswordService _passwordService;

    public EntidadFiscalService(IEntidadFiscalRepository repository, IPasswordService passwordService)
    {
        _repository = repository;
        _passwordService = passwordService;
    }

    public async Task<IReadOnlyCollection<EntidadFiscalDto>> ObtenerTodosAsync(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<EntidadFiscal> entities = await _repository.ObtenerTodosAsync(cancellationToken);
        return entities.Select(Mapear).ToArray();
    }

    public async Task<EntidadFiscalDto?> ObtenerPorGuidAsync(string guid, CancellationToken cancellationToken)
    {
        EntidadFiscal? entity = await _repository.ObtenerPorGuidAsync(guid, false, cancellationToken);

        return entity is null ? null : Mapear(entity);
    }

    public async Task<CrearEntidadFiscalDto> CrearAsync(
        CrearEntidadFiscalCommandDto request,
        CancellationToken cancellationToken)
    {
        if (await _repository.ExisteRedComercialAsync(request.NombreRedComercial, cancellationToken))
        {
            throw new ConflictoException("Ya existe una red comercial registrada con el nombre proporcionado.");
        }

        if (await _repository.ExisteEmailAsync(request.Email, cancellationToken))
        {
            throw new ConflictoException("Ya existe una cuenta registrada con el correo proporcionado.");
        }

        Rol role = await _repository.ObtenerRolPorNombreAsync(SuperAdministradorRole, cancellationToken)
            ?? throw new EntidadNoEncontradaException(
                "No se encontró el rol SUPERADMIN. Verifique que RolesSeeder haya sido aplicado.");

        DateTime now = DateTime.UtcNow;
        string temporaryPassword = _passwordService.GenerarPasswordTemporal();

        RedComercial redComercial = new()
        {
            NombreRedComercial = request.NombreRedComercial.Trim(),
            Guid = Guid.NewGuid().ToString(),
            CreatedAt = now,
            UpdatedAt = now
        };

        EntidadFiscal entidadFiscal = new()
        {
            SatRegimenFiscalId = 1,
            RazonSocial = "PENDIENTE DE CONFIGURAR",
            Rfc = $"PEND{Guid.NewGuid():N}"[..13].ToUpperInvariant(),
            CodigoPostal = "00000",
            TimbresDisponibles = 0,
            EsPrepago = true,
            Guid = Guid.NewGuid().ToString(),
            CreatedAt = now,
            UpdatedAt = now
        };

        RedComercialEntidadFiscal relation = new()
        {
            RedComercial = redComercial,
            EntidadFiscal = entidadFiscal,
            Guid = Guid.NewGuid().ToString(),
            CreatedAt = now,
            UpdatedAt = now
        };

        Accounts account = new()
        {
            RolId = role.Id,
            RedComercial = redComercial,
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Telefono = request.Telefono.Trim(),
            PasswordExpiresAt = now,
            Guid = Guid.NewGuid().ToString(),
            CreatedAt = now,
            UpdatedAt = now
        };
        account.Password = _passwordService.CrearHash(account, temporaryPassword);

        await _repository.CrearRegistroInicialAsync(
            redComercial,
            entidadFiscal,
            relation,
            account,
            cancellationToken);

        return new CrearEntidadFiscalDto
        {
            EntidadFiscal = Mapear(entidadFiscal),
            PasswordTemporal = temporaryPassword,
            PasswordExpiresAt = account.PasswordExpiresAt
        };
    }

    public async Task<EntidadFiscalDto?> ActualizarAsync(
        string guid,
        ActualizarEntidadFiscalDto request,
        CancellationToken cancellationToken)
    {
        EntidadFiscal? entity = await _repository.ObtenerPorGuidAsync(guid, true, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        if (request.SatRegimenFiscalId.HasValue)
        {
            if (!await _repository.ExisteRegimenFiscalAsync(request.SatRegimenFiscalId.Value, cancellationToken))
            {
                throw new EntidadNoEncontradaException("El régimen fiscal indicado no existe.");
            }
        }

        entity.SatRegimenFiscalId = request.SatRegimenFiscalId;
        if (request.EsPrepago.HasValue)
        {
            entity.EsPrepago = request.EsPrepago.Value;
        }
        entity.RazonSocial = request.RazonSocial.Trim();
        entity.Rfc = request.Rfc.Trim().ToUpperInvariant();
        entity.RegistroPatronal = request.RegistroPatronal;
        entity.Calle = request.Calle;
        entity.NumExt = request.NumExt;
        entity.NumInt = request.NumInt;
        entity.Colonia = request.Colonia;
        entity.Ciudad = request.Ciudad;
        entity.Estado = request.Estado;
        entity.CodigoPostal = request.CodigoPostal;
        entity.NombreComercial = request.NombreComercial;
        entity.TelefonoComercial = request.TelefonoComercial;
        entity.TelefonoComercial2 = request.TelefonoComercial2;
        entity.CorreoComercial = request.CorreoComercial;
        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.GuardarCambiosAsync(cancellationToken);
        return Mapear(entity);
    }

    public async Task<bool> EliminarAsync(string guid, CancellationToken cancellationToken)
    {
        EntidadFiscal? entity = await _repository.ObtenerPorGuidAsync(guid, true, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        await _repository.EliminarAsync(entity, cancellationToken);
        return true;
    }

    private static EntidadFiscalDto Mapear(EntidadFiscal entity)
    {
        return new EntidadFiscalDto
        {
            Guid = entity.Guid,
            SatRegimenFiscalId = entity.SatRegimenFiscalId,
            RazonSocial = entity.RazonSocial,
            Rfc = entity.Rfc,
            RegistroPatronal = entity.RegistroPatronal,
            Calle = entity.Calle,
            NumExt = entity.NumExt,
            NumInt = entity.NumInt,
            Colonia = entity.Colonia,
            Ciudad = entity.Ciudad,
            Estado = entity.Estado,
            CodigoPostal = entity.CodigoPostal,
            TimbresDisponibles = entity.TimbresDisponibles,
            EsPrepago = entity.EsPrepago,
            NombreComercial = entity.NombreComercial,
            TelefonoComercial = entity.TelefonoComercial,
            TelefonoComercial2 = entity.TelefonoComercial2,
            CorreoComercial = entity.CorreoComercial,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

}
