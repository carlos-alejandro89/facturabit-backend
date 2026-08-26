using FacturaBit.Application.DTOs;

namespace FacturaBit.Application.Abstractions;

public interface ISatRegimenFiscalService
{
    Task<IReadOnlyCollection<SatRegimenFiscalDto>> ObtenerTodosAsync(CancellationToken cancellationToken);
}
