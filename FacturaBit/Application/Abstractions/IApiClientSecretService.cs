using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Abstractions;

public interface IApiClientSecretService
{
    string GenerarClientId();
    string GenerarClientSecret();
    string CrearHash(ApiClient apiClient, string secret);
    bool Verificar(ApiClient apiClient, string secret);
}
