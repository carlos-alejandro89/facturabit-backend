using FacturaBit.Application.DTOs;

namespace FacturaBit.Application.Abstractions;

public interface ICertificadoDigitalService
{
    Task<IReadOnlyCollection<CertificadoDigitalDto>?> ObtenerPorEmisorAsync(
        string rfc,
        string redComercialGuid,
        CancellationToken cancellationToken);
}
