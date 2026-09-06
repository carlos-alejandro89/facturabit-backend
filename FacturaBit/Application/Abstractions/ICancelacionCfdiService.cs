using FacturaBit.Application.DTOs;

namespace FacturaBit.Application.Abstractions;

public interface ICancelacionCfdiService
{
    Task<CancelacionResultDto> CancelarAsync(
        CancelacionDto cancelacion,
        string redComercialGuid,
        CancellationToken cancellationToken);
}
