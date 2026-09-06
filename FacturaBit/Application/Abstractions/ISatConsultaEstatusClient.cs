using FacturaBit.Application.DTOs;

namespace FacturaBit.Application.Abstractions;

public interface ISatConsultaEstatusClient
{
    Task<SatConsultaEstatusResultDto> ConsultarAsync(
        SatConsultaEstatusRequestDto request,
        CancellationToken cancellationToken);
}
