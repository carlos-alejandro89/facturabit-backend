using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Abstractions;

public interface IApiClientRepository
{
    Task<RedComercial?> ObtenerRedComercialAsync(string redComercialGuid, CancellationToken cancellationToken);
    Task<bool> TieneAccesoApiAsync(int redComercialId, CancellationToken cancellationToken);
    Task<bool> ExisteClientIdAsync(string clientId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ApiClient>> ObtenerPorRedAsync(int redComercialId, CancellationToken cancellationToken);
    Task<ApiClient?> ObtenerPorGuidAsync(string guid, int redComercialId, bool tracking, CancellationToken cancellationToken);
    Task<ApiClient?> ObtenerPorClientIdAsync(string clientId, bool tracking, CancellationToken cancellationToken);
    Task<bool> EmisorPerteneceAsync(int redComercialId, string rfc, CancellationToken cancellationToken);
    Task AgregarAsync(ApiClient apiClient, CancellationToken cancellationToken);
    Task GuardarCambiosAsync(CancellationToken cancellationToken);
}
