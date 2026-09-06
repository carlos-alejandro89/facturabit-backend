using FacturaBit.Application.DTOs;

namespace FacturaBit.Application.Abstractions;

public interface IConsultaEstatusCfdiService
{
    Task<ConsultaEstatusCfdiResultDto> ConsultarAsync(
        string rfc,
        string uuid,
        string redComercialGuid,
        CancellationToken cancellationToken);
}
