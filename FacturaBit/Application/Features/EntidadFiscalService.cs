using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Application.Exceptions;
using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Features;

public sealed class EntidadFiscalService : IEntidadFiscalService
{
    private const string SuperAdministradorRole = "SUPERADMIN";
    private const int EnProcesoStatusId = 5;
    private const string EnProcesoStatusGuid = "c50f34f6-3a40-4d1c-b304-1504b324c6de";
    private const decimal IvaRate = 0.16m;
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

    public async Task<IReadOnlyCollection<EntidadFiscalDto>> ObtenerPorRedComercialAsync(
        string redComercialGuid,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<EntidadFiscal> entities = await _repository.ObtenerPorRedComercialGuidAsync(
            redComercialGuid,
            cancellationToken);
        return entities.Select(Mapear).ToArray();
    }

    public async Task<EntidadFiscalDto?> ObtenerPorGuidAsync(
        string guid,
        string redComercialGuid,
        CancellationToken cancellationToken)
    {
        EntidadFiscal? entity = await _repository.ObtenerPorGuidYRedComercialGuidAsync(
            guid,
            redComercialGuid,
            false,
            cancellationToken);
        return entity is null ? null : Mapear(entity);
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

        Suscripcion suscripcion = await _repository.ObtenerSuscripcionPorGuidAsync(
                request.SuscripcionGuid,
                cancellationToken)
            ?? throw new EntidadNoEncontradaException(
                "La suscripción seleccionada no existe o ya no está disponible.");

        Estatus enProceso = await _repository.ObtenerEstatusPorIdYGuidAsync(
                EnProcesoStatusId,
                EnProcesoStatusGuid,
                cancellationToken)
            ?? throw new EntidadNoEncontradaException(
                "No se encontró el estatus EN PROCESO. Verifique que EstatusSeeder haya sido aplicado.");

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

        decimal total = suscripcion.Precio;
        decimal subtotal = Math.Round(total / (1m + IvaRate), 2, MidpointRounding.AwayFromZero);
        decimal iva = total - subtotal;
        string orderToken = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

        OrdenCompraSuscripcion ordenCompra = new()
        {
            Folio = $"OC-{now:yyyyMMddHHmmss}-{orderToken}",
            RedComercial = redComercial,
            Estatus = enProceso,
            Suscripcion = suscripcion,
            CanalId = null,
            Subtotal = subtotal,
            Iva = iva,
            Total = total,
            Guid = Guid.NewGuid().ToString(),
            CreatedAt = now,
            UpdatedAt = now
        };

        await _repository.CrearRegistroInicialAsync(
            redComercial,
            entidadFiscal,
            relation,
            account,
            ordenCompra,
            cancellationToken);

        return new CrearEntidadFiscalDto
        {
            EntidadFiscal = Mapear(entidadFiscal),
            PasswordTemporal = temporaryPassword,
            PasswordExpiresAt = account.PasswordExpiresAt,
            OrdenCompraGuid = ordenCompra.Guid,
            FolioOrdenCompra = ordenCompra.Folio
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

    public async Task<EntidadFiscalDto> UpsertAsync(
        string redComercialGuid,
        UpsertEntidadFiscalDto request,
        CancellationToken cancellationToken)
    {
        if (request.SatRegimenFiscalId.HasValue
            && !await _repository.ExisteRegimenFiscalAsync(request.SatRegimenFiscalId.Value, cancellationToken))
        {
            throw new EntidadNoEncontradaException("El régimen fiscal indicado no existe.");
        }

        if (await _repository.ExisteRfcEnOtraEntidadAsync(request.Rfc, request.Guid, cancellationToken))
        {
            throw new ConflictoException("Ya existe un emisor registrado con el RFC proporcionado.");
        }

        DateTime now = DateTime.UtcNow;
        EntidadFiscal? entity = null;
        if (!string.IsNullOrWhiteSpace(request.Guid))
        {
            entity = await _repository.ObtenerPorGuidYRedComercialGuidAsync(
                request.Guid,
                redComercialGuid,
                true,
                cancellationToken);

            if (entity is null)
            {
                throw new EntidadNoEncontradaException("El emisor no existe o no pertenece a la organización actual.");
            }
        }

        if (entity is null)
        {
            RedComercial redComercial = await _repository.ObtenerRedComercialPorGuidAsync(
                    redComercialGuid,
                    cancellationToken)
                ?? throw new EntidadNoEncontradaException("La organización asociada a la sesión no existe.");

            entity = new EntidadFiscal
            {
                Guid = Guid.NewGuid().ToString(),
                TimbresDisponibles = 0,
                EsPrepago = request.EsPrepago ?? true,
                RazonSocial = string.Empty,
                Rfc = string.Empty,
                CodigoPostal = string.Empty,
                CreatedAt = now,
                UpdatedAt = now
            };

            AplicarCambios(entity, request, now);
            RedComercialEntidadFiscal relation = new()
            {
                RedComercial = redComercial,
                EntidadFiscal = entity,
                Guid = Guid.NewGuid().ToString(),
                CreatedAt = now,
                UpdatedAt = now
            };
            await _repository.AgregarEntidadFiscalAsync(entity, relation, cancellationToken);
        }
        else
        {
            AplicarCambios(entity, request, now);
            await _repository.GuardarCambiosAsync(cancellationToken);
        }

        EntidadFiscal? saved = await _repository.ObtenerPorGuidYRedComercialGuidAsync(
            entity.Guid,
            redComercialGuid,
            false,
            cancellationToken);
        return Mapear(saved ?? entity);
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

    public async Task<bool> EliminarAsync(
        string guid,
        string redComercialGuid,
        CancellationToken cancellationToken)
    {
        EntidadFiscal? entity = await _repository.ObtenerPorGuidYRedComercialGuidAsync(
            guid,
            redComercialGuid,
            true,
            cancellationToken);
        if (entity is null) return false;

        await _repository.EliminarAsync(entity, cancellationToken);
        return true;
    }

    private static EntidadFiscalDto Mapear(EntidadFiscal entity)
    {
        return new EntidadFiscalDto
        {
            Guid = entity.Guid,
            SatRegimenFiscalId = entity.SatRegimenFiscalId,
            SatRegimenFiscalClave = entity.SatRegimenFiscal?.CveRegimen,
            SatRegimenFiscalNombre = entity.SatRegimenFiscal?.RegimenFiscal,
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
            CertificadoVigente = entity.Certificados.Any(certificado =>
                certificado.ValidoDesde <= DateTime.UtcNow && certificado.ValidoHasta >= DateTime.UtcNow),
            NombreComercial = entity.NombreComercial,
            TelefonoComercial = entity.TelefonoComercial,
            TelefonoComercial2 = entity.TelefonoComercial2,
            CorreoComercial = entity.CorreoComercial,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private static void AplicarCambios(EntidadFiscal entity, UpsertEntidadFiscalDto request, DateTime now)
    {
        entity.SatRegimenFiscalId = request.SatRegimenFiscalId;
        entity.EsPrepago = request.EsPrepago ?? entity.EsPrepago;
        entity.RazonSocial = request.RazonSocial.Trim().ToUpperInvariant();
        entity.Rfc = request.Rfc.Trim().ToUpperInvariant();
        entity.RegistroPatronal = Limpiar(request.RegistroPatronal);
        entity.Calle = Limpiar(request.Calle);
        entity.NumExt = Limpiar(request.NumExt);
        entity.NumInt = Limpiar(request.NumInt);
        entity.Colonia = Limpiar(request.Colonia);
        entity.Ciudad = Limpiar(request.Ciudad);
        entity.Estado = Limpiar(request.Estado);
        entity.CodigoPostal = request.CodigoPostal.Trim();
        entity.NombreComercial = Limpiar(request.NombreComercial);
        entity.TelefonoComercial = Limpiar(request.TelefonoComercial);
        entity.TelefonoComercial2 = Limpiar(request.TelefonoComercial2);
        entity.CorreoComercial = Limpiar(request.CorreoComercial)?.ToLowerInvariant();
        entity.UpdatedAt = now;
    }

    private static string? Limpiar(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

}
