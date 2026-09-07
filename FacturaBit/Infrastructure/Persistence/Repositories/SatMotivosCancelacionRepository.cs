using FacturaBit.Application.Abstractions;
using FacturaBit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FacturaBit.Infrastructure.Persistence.Repositories;

public sealed class SatMotivosCancelacionRepository : ISatMotivosCancelacionRepository
{
    private readonly AppDBContext _dbContext;

    public SatMotivosCancelacionRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<SatMotivosCancelacion>> ObtenerTodosAsync(
        CancellationToken cancellationToken)
    {
        return await _dbContext.Set<SatMotivosCancelacion>()
            .AsNoTracking()
            .OrderBy(motivo => motivo.CveMotivo)
            .ToArrayAsync(cancellationToken);
    }
}
