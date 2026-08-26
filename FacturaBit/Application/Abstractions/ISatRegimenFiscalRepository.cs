using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Abstractions;

public interface ISatRegimenFiscalRepository
{
    Task<IReadOnlyCollection<SatRegimenFiscal>> ObtenerTodosAsync(CancellationToken cancellationToken);
}
