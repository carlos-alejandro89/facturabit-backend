using FacturaBit.Application.DTOs;

namespace FacturaBit.Application.Abstractions;

public interface IAuthenticationService
{
    Task<LoginResponseDto?> LoginAsync(string email, string password, CancellationToken cancellationToken);
}
