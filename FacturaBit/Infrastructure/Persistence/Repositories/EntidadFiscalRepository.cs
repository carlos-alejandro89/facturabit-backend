using FacturaBit.Application.Abstractions;
using FacturaBit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FacturaBit.Infrastructure.Persistence.Repositories;

public sealed class EntidadFiscalRepository : IEntidadFiscalRepository
{
    private readonly AppDBContext _dbContext;

    public EntidadFiscalRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<EntidadFiscal>> ObtenerTodosAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Set<EntidadFiscal>()
            .AsNoTracking()
            .OrderBy(entity => entity.RazonSocial)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<EntidadFiscal>> ObtenerPorRedComercialGuidAsync(
        string redComercialGuid,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Set<EntidadFiscal>()
            .AsNoTracking()
            .Include(entity => entity.SatRegimenFiscal)
            .Include(entity => entity.Certificados)
            .Where(entity => entity.RedesComerciales.Any(relation => relation.RedComercial.Guid == redComercialGuid))
            .OrderBy(entity => entity.RazonSocial)
            .ToArrayAsync(cancellationToken);
    }

    public Task<EntidadFiscal?> ObtenerPorGuidYRedComercialGuidAsync(
        string guid,
        string redComercialGuid,
        bool tracking,
        CancellationToken cancellationToken)
    {
        IQueryable<EntidadFiscal> query = _dbContext.Set<EntidadFiscal>()
            .Include(entity => entity.SatRegimenFiscal)
            .Include(entity => entity.Certificados)
            .Where(entity => entity.Guid == guid
                && entity.RedesComerciales.Any(relation => relation.RedComercial.Guid == redComercialGuid));

        return (tracking ? query : query.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
    }

    public Task<RedComercial?> ObtenerRedComercialPorGuidAsync(string guid, CancellationToken cancellationToken)
    {
        return _dbContext.Set<RedComercial>().SingleOrDefaultAsync(red => red.Guid == guid, cancellationToken);
    }

    public Task<bool> ExisteRfcEnOtraEntidadAsync(
        string rfc,
        string? entidadFiscalGuid,
        CancellationToken cancellationToken)
    {
        string normalizedRfc = rfc.Trim().ToUpperInvariant();
        return _dbContext.Set<EntidadFiscal>().AnyAsync(
            entity => entity.Rfc.ToUpper() == normalizedRfc && entity.Guid != entidadFiscalGuid,
            cancellationToken);
    }

    public async Task<EntidadFiscal?> ObtenerPorGuidAsync(
        string guid,
        bool incluirRelaciones,
        CancellationToken cancellationToken)
    {
        IQueryable<EntidadFiscal> query = _dbContext.Set<EntidadFiscal>();
        if (incluirRelaciones)
        {
            query = query.Include(entity => entity.RedesComerciales);
        }
        else
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(entity => entity.Guid == guid, cancellationToken);
    }

    public Task<EntidadFiscal?> ObtenerPorRfcAsync(string rfc, CancellationToken cancellationToken)
    {
        string normalizedRfc = rfc.Trim().ToUpperInvariant();
        return _dbContext.Set<EntidadFiscal>()
            .AsNoTracking()
            .SingleOrDefaultAsync(entity => entity.Rfc.ToUpper() == normalizedRfc, cancellationToken);
    }

    public Task<bool> ExisteEmailAsync(string email, CancellationToken cancellationToken)
    {
        string normalizedEmail = email.Trim().ToUpperInvariant();
        return _dbContext.Set<Accounts>()
            .AnyAsync(account => account.Email.ToUpper() == normalizedEmail, cancellationToken);
    }

    public Task<bool> ExisteRedComercialAsync(string nombre, CancellationToken cancellationToken)
    {
        string normalizedName = nombre.Trim().ToUpperInvariant();
        return _dbContext.Set<RedComercial>()
            .AnyAsync(red => red.NombreRedComercial.ToUpper() == normalizedName, cancellationToken);
    }

    public Task<Rol?> ObtenerRolPorNombreAsync(string nombre, CancellationToken cancellationToken)
    {
        return _dbContext.Set<Rol>().SingleOrDefaultAsync(role => role.Nombre == nombre, cancellationToken);
    }

    public Task<Suscripcion?> ObtenerSuscripcionPorGuidAsync(string guid, CancellationToken cancellationToken)
    {
        string normalizedGuid = guid.Trim().ToLowerInvariant();
        return _dbContext.Set<Suscripcion>()
            .SingleOrDefaultAsync(item => item.Guid.ToLower() == normalizedGuid, cancellationToken);
    }

    public Task<Estatus?> ObtenerEstatusPorIdYGuidAsync(
        int id,
        string guid,
        CancellationToken cancellationToken)
    {
        string normalizedGuid = guid.Trim().ToLowerInvariant();
        return _dbContext.Set<Estatus>()
            .SingleOrDefaultAsync(
                item => item.Id == id && item.Guid.ToLower() == normalizedGuid,
                cancellationToken);
    }

    public Task<bool> ExisteRegimenFiscalAsync(int id, CancellationToken cancellationToken)
    {
        return _dbContext.Set<SatRegimenFiscal>().AnyAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<bool> DescontarTimbrePorRfcAsync(string rfc, CancellationToken cancellationToken)
    {
        string normalizedRfc = rfc.Trim().ToUpperInvariant();
        DateTime utcNow = DateTime.UtcNow;
        int affectedRows = await _dbContext.Set<EntidadFiscal>()
            .Where(entity => entity.Rfc.ToUpper() == normalizedRfc
                && entity.EsPrepago
                && entity.TimbresDisponibles > 0)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(
                    entity => entity.TimbresDisponibles,
                    entity => entity.TimbresDisponibles - 1)
                .SetProperty(entity => entity.UpdatedAt, utcNow),
                cancellationToken);

        return affectedRows == 1;
    }

    public async Task CrearRegistroInicialAsync(
        RedComercial redComercial,
        EntidadFiscal entidadFiscal,
        RedComercialEntidadFiscal relacion,
        Accounts account,
        OrdenCompraSuscripcion ordenCompra,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        _dbContext.AddRange(redComercial, entidadFiscal, relacion, account, ordenCompra);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public Task GuardarCambiosAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AgregarEntidadFiscalAsync(
        EntidadFiscal entidadFiscal,
        RedComercialEntidadFiscal relacion,
        CancellationToken cancellationToken)
    {
        _dbContext.AddRange(entidadFiscal, relacion);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task EliminarAsync(EntidadFiscal entidadFiscal, CancellationToken cancellationToken)
    {
        _dbContext.RemoveRange(entidadFiscal.RedesComerciales);
        _dbContext.Remove(entidadFiscal);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
