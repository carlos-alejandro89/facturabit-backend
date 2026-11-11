using FacturaBit.Application.DTOs;

namespace FacturaBit.Application.Abstractions;

public interface IComplementoPagoService
{
    Task<FacturacionResultDto> EmitirAsync(
        ComplementoPagoDto complemento,
        CancellationToken cancellationToken);
}
