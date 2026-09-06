using FacturaBit.Application.DTOs;

namespace FacturaBit.Application.Abstractions;

public interface IFacturacionService
{
    Task<FacturacionResultDto> EmitirAsync(InvoiceModelDto invoice, bool esGlobal, CancellationToken cancellationToken);
}
