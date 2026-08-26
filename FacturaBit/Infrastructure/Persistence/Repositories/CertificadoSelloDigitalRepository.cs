using FacturaBit.Application.Abstractions;
using FacturaBit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FacturaBit.Infrastructure.Persistence.Repositories;

public sealed class CertificadoSelloDigitalRepository : ICertificadoSelloDigitalRepository
{
    private readonly AppDBContext _dbContext;

    public CertificadoSelloDigitalRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<EntidadFiscal?> ObtenerEntidadFiscalPorRfcAsync(string rfc, CancellationToken cancellationToken)
    {
        return _dbContext.Set<EntidadFiscal>()
            .SingleOrDefaultAsync(entity => entity.Rfc == rfc, cancellationToken);
    }

    public Task<EntidadFiscal?> ObtenerEntidadFiscalPorRfcYRedComercialAsync(
        string rfc,
        string redComercialGuid,
        CancellationToken cancellationToken)
    {
        return _dbContext.Set<EntidadFiscal>().SingleOrDefaultAsync(
            entity => entity.Rfc == rfc
                && entity.RedesComerciales.Any(relation => relation.RedComercial.Guid == redComercialGuid),
            cancellationToken);
    }

    public Task<CertificadoSelloDigital?> ObtenerVigentePorRfcAsync(
        string rfc,
        CancellationToken cancellationToken)
    {
        DateTime utcNow = DateTime.UtcNow;
        return _dbContext.Set<CertificadoSelloDigital>()
            .AsNoTracking()
            .Where(item => item.EntidadFiscal.Rfc == rfc
                && item.ValidoDesde <= utcNow
                && item.ValidoHasta >= utcNow)
            .OrderByDescending(item => item.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<CertificadoSelloDigital>?> ObtenerPorRfcYRedComercialAsync(
        string rfc,
        string redComercialGuid,
        CancellationToken cancellationToken)
    {
        EntidadFiscal? entidadFiscal = await _dbContext.Set<EntidadFiscal>()
            .AsNoTracking()
            .SingleOrDefaultAsync(entity => entity.Rfc == rfc
                && entity.RedesComerciales.Any(relation => relation.RedComercial.Guid == redComercialGuid),
                cancellationToken);
        if (entidadFiscal is null) return null;

        return await _dbContext.Set<CertificadoSelloDigital>()
            .AsNoTracking()
            .Where(certificado => certificado.EntidadFiscalId == entidadFiscal.Id)
            .OrderByDescending(certificado => certificado.ValidoHasta)
            .ToArrayAsync(cancellationToken);
    }

    public async Task InsertarAsync(CertificadoSelloDigital certificado, CancellationToken cancellationToken)
    {
        _dbContext.Add(certificado);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
