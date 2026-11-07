using FacturaBit.Application.DTOs;

namespace FacturaBit.Application.Abstractions;

public interface IComprobanteFiscalService
{
    Task<ComprobantesPageDto> ObtenerAsync(
        string redComercialGuid,
        string? emisorGuid,
        string? busqueda,
        string? estatus,
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken);
    Task<ComprobanteXmlDto?> ObtenerXmlAsync(string guid, string redComercialGuid, CancellationToken cancellationToken);
    Task<ComprobanteArchivoDto?> ObtenerPdfAsync(string guid, string redComercialGuid, CancellationToken cancellationToken);
    Task<ComprobanteArchivoDto?> ObtenerAcusePdfAsync(string guid, string redComercialGuid, CancellationToken cancellationToken);
}
