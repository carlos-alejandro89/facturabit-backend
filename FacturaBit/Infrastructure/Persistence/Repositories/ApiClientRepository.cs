using FacturaBit.Application.Abstractions;
using FacturaBit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FacturaBit.Infrastructure.Persistence.Repositories;

public sealed class ApiClientRepository : IApiClientRepository
{
    private readonly AppDBContext _db;
    public ApiClientRepository(AppDBContext db) => _db = db;

    public Task<RedComercial?> ObtenerRedComercialAsync(string redComercialGuid, CancellationToken cancellationToken)
        => _db.Set<RedComercial>().SingleOrDefaultAsync(item => item.Guid == redComercialGuid, cancellationToken);

    public async Task<bool> TieneAccesoApiAsync(int redComercialId, CancellationToken cancellationToken)
    {
        OrdenCompraSuscripcion[] orders = await _db.Set<OrdenCompraSuscripcion>()
            .AsNoTracking()
            .Include(order => order.Suscripcion)
            .Where(order => order.RedComercialId == redComercialId && order.EstatusId == 9)
            .ToArrayAsync(cancellationToken);

        return orders.Any(order => order.Suscripcion.Caracteristicas.AccesoApi);
    }

    public Task<bool> ExisteClientIdAsync(string clientId, CancellationToken cancellationToken)
        => _db.Set<ApiClient>().AnyAsync(item => item.ClientId == clientId, cancellationToken);

    public async Task<IReadOnlyCollection<ApiClient>> ObtenerPorRedAsync(int redComercialId, CancellationToken cancellationToken)
        => await _db.Set<ApiClient>().AsNoTracking().Where(item => item.RedComercialId == redComercialId)
            .OrderByDescending(item => item.CreatedAt).ToArrayAsync(cancellationToken);

    public Task<ApiClient?> ObtenerPorGuidAsync(string guid, int redComercialId, bool tracking, CancellationToken cancellationToken)
    {
        IQueryable<ApiClient> query = _db.Set<ApiClient>().Where(item => item.Guid == guid && item.RedComercialId == redComercialId);
        return (tracking ? query : query.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
    }

    public Task<ApiClient?> ObtenerPorClientIdAsync(string clientId, bool tracking, CancellationToken cancellationToken)
    {
        IQueryable<ApiClient> query = _db.Set<ApiClient>().Include(item => item.RedComercial).Where(item => item.ClientId == clientId);
        return (tracking ? query : query.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
    }

    public Task<bool> EmisorPerteneceAsync(int redComercialId, string rfc, CancellationToken cancellationToken)
    {
        string normalized = rfc.Trim().ToUpperInvariant();
        return _db.Set<RedComercialEntidadFiscal>().AnyAsync(relation => relation.RedComercialId == redComercialId
            && relation.EntidadFiscal.Rfc.ToUpper() == normalized, cancellationToken);
    }

    public async Task AgregarAsync(ApiClient apiClient, CancellationToken cancellationToken)
    {
        _db.Add(apiClient);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task GuardarCambiosAsync(CancellationToken cancellationToken) => _db.SaveChangesAsync(cancellationToken);
}
