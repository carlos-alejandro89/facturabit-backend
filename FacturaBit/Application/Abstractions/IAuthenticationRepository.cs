using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Abstractions;

public interface IAuthenticationRepository
{
    Task<Accounts?> ObtenerPorEmailAsync(string email, CancellationToken cancellationToken);
}
