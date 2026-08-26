using FacturaBit.Domain.Entities;
using FacturaBit.Application.DTOs;

namespace FacturaBit.Application.Abstractions;

public interface IComprobanteFiscalRepository
{
    Task<(IReadOnlyCollection<ComprobanteFiscal> Items, int Total)> ObtenerPorRedComercialAsync(
        string redComercialGuid,
        string? emisorGuid,
        string? busqueda,
        string? estatus,
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken);
    Task<ComprobanteFiscal?> ObtenerPorGuidYRedComercialAsync(
        string guid,
        string redComercialGuid,
        CancellationToken cancellationToken);
    Task InsertarAsync(ComprobanteFiscal comprobante, CancellationToken cancellationToken);
    Task MarcarErrorAsync(int id, string mensaje, CancellationToken cancellationToken);
    Task MarcarVigenteAsync(
        int id,
        FacturacionResultDto resultado,
        string mensaje,
        string? pathXml,
        CancellationToken cancellationToken);
}
