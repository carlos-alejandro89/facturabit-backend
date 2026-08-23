using FacturaBit.Domain.Entities;
using FacturaBit.Application.DTOs;

namespace FacturaBit.Application.Abstractions;

public interface IComprobanteFiscalRepository
{
    Task InsertarAsync(ComprobanteFiscal comprobante, CancellationToken cancellationToken);
    Task MarcarErrorAsync(int id, string mensaje, CancellationToken cancellationToken);
    Task MarcarVigenteAsync(
        int id,
        FacturacionResultDto resultado,
        string mensaje,
        string? pathXml,
        CancellationToken cancellationToken);
}
