using FacturaBit.Application.DTOs;

namespace FacturaBit.Application.Abstractions;

public interface ISatMotivosCancelacionService
{
    Task<IReadOnlyCollection<SatMotivoCancelacionDto>> ObtenerTodosAsync(
        CancellationToken cancellationToken);
}
