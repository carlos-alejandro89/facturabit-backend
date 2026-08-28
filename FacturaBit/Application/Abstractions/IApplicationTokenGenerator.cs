using FacturaBit.Application.DTOs;
using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Abstractions;

public interface IApplicationTokenGenerator
{
    ClientAccessTokenDto Generar(ApiClient apiClient, string scope);
}
