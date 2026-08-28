using FacturaBit.Application.DTOs;

namespace FacturaBit.Application.Abstractions;

public interface IApiClientService
{
    Task<ApiClientCreatedDto> CrearAsync(string redComercialGuid, string nombre, IReadOnlyCollection<string>? scopes, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ApiClientDto>> ObtenerAsync(string redComercialGuid, CancellationToken cancellationToken);
    Task<bool> RevocarAsync(string redComercialGuid, string guid, CancellationToken cancellationToken);
    Task<ClientAccessTokenDto?> AutenticarAsync(string clientId, string clientSecret, string? requestedScope, CancellationToken cancellationToken);
    Task<bool> PuedeEmitirAsync(string redComercialId, string rfcEmisor, CancellationToken cancellationToken);
}
