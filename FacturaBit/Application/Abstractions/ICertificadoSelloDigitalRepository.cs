using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Abstractions;

public interface ICertificadoSelloDigitalRepository
{
    Task<EntidadFiscal?> ObtenerEntidadFiscalPorRfcAsync(string rfc, CancellationToken cancellationToken);
    Task<EntidadFiscal?> ObtenerEntidadFiscalPorRfcYRedComercialAsync(
        string rfc,
        string redComercialGuid,
        CancellationToken cancellationToken);
    Task<CertificadoSelloDigital?> ObtenerVigentePorRfcAsync(string rfc, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<CertificadoSelloDigital>?> ObtenerPorRfcYRedComercialAsync(
        string rfc,
        string redComercialGuid,
        CancellationToken cancellationToken);
    Task InsertarAsync(CertificadoSelloDigital certificado, CancellationToken cancellationToken);
}
