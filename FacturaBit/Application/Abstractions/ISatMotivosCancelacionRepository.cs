using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Abstractions;

public interface ISatMotivosCancelacionRepository
{
    Task<IReadOnlyCollection<SatMotivosCancelacion>> ObtenerTodosAsync(
        CancellationToken cancellationToken);
}
