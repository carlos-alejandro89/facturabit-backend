using FacturaBit.Application.DTOs;

namespace FacturaBit.Application.Abstractions;

public interface IEntidadFiscalService
{
    Task<IReadOnlyCollection<EntidadFiscalDto>> ObtenerTodosAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<EntidadFiscalDto>> ObtenerPorRedComercialAsync(string redComercialGuid, CancellationToken cancellationToken);
    Task<EntidadFiscalDto?> ObtenerPorGuidAsync(string guid, string redComercialGuid, CancellationToken cancellationToken);
    Task<EntidadFiscalDto?> ObtenerPorGuidAsync(string guid, CancellationToken cancellationToken);
    Task<CrearEntidadFiscalDto> CrearAsync(CrearEntidadFiscalCommandDto request, CancellationToken cancellationToken);
    Task<EntidadFiscalDto?> ActualizarAsync(string guid, ActualizarEntidadFiscalDto request, CancellationToken cancellationToken);
    Task<EntidadFiscalDto> UpsertAsync(string redComercialGuid, UpsertEntidadFiscalDto request, CancellationToken cancellationToken);
    Task<bool> EliminarAsync(string guid, CancellationToken cancellationToken);
    Task<bool> EliminarAsync(string guid, string redComercialGuid, CancellationToken cancellationToken);
}
