using FacturaBit.Application.DTOs;

namespace FacturaBit.Application.Abstractions;

public interface ITimbradoService
{
    Task<TimbradoResultDto> TimbrarAsync(string xml, CancellationToken cancellationToken);
    Task<ProveedorCancelacionResultDto> CancelarAsync(CancelacionDto cancelacion, CancellationToken cancellationToken);
}
