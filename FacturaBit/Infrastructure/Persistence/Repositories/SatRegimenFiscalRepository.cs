using FacturaBit.Application.Abstractions;
using FacturaBit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FacturaBit.Infrastructure.Persistence.Repositories;

public sealed class SatRegimenFiscalRepository : ISatRegimenFiscalRepository
{
    private readonly AppDBContext _dbContext;

    public SatRegimenFiscalRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<SatRegimenFiscal>> ObtenerTodosAsync(
        CancellationToken cancellationToken)
    {
        return await _dbContext.Set<SatRegimenFiscal>()
            .AsNoTracking()
            .OrderBy(regimen => regimen.CveRegimen)
            .ToArrayAsync(cancellationToken);
    }
}
