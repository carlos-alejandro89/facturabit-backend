using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Abstractions;

public interface ICertificadoSelloDigitalRepository
{
    Task<EntidadFiscal?> ObtenerEntidadFiscalPorRfcAsync(string rfc, CancellationToken cancellationToken);
    Task<CertificadoSelloDigital?> ObtenerVigentePorRfcAsync(string rfc, CancellationToken cancellationToken);
    Task InsertarAsync(CertificadoSelloDigital certificado, CancellationToken cancellationToken);
}
