using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Features;

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationRepository _repository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthenticationService(
        IAuthenticationRepository repository,
        IPasswordService passwordService,
        IJwtTokenGenerator tokenGenerator)
    {
        _repository = repository;
        _passwordService = passwordService;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<LoginResponseDto?> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        Accounts? account = await _repository.ObtenerPorEmailAsync(email, cancellationToken);
        if (account is null || string.IsNullOrWhiteSpace(account.Password)
            || !_passwordService.Verificar(account, password))
        {
            return null;
        }

        bool requiereCambioPassword = account.PasswordExpiresAt <= DateTime.UtcNow;
        GeneratedTokenDto generatedToken = _tokenGenerator.Generar(account, requiereCambioPassword);

        return new LoginResponseDto
        {
            Token = generatedToken.Token,
            ExpiresAt = generatedToken.ExpiresAt,
            AccountGuid = account.Guid,
            RedComercial = new RedComercialLoginDto
            {
                Guid = account.RedComercial.Guid,
                NombreRedComercial = account.RedComercial.NombreRedComercial
            },
            Email = account.Email,
            FullName = account.FullName,
            Rol = account.Rol.Nombre,
            RequiereCambioPassword = requiereCambioPassword
        };
    }
}
