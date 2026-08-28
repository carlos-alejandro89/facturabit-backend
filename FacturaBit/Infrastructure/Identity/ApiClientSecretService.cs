using System.Security.Cryptography;
using FacturaBit.Application.Abstractions;
using FacturaBit.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FacturaBit.Infrastructure.Identity;

public sealed class ApiClientSecretService : IApiClientSecretService
{
    private readonly IPasswordHasher<ApiClient> _hasher;
    public ApiClientSecretService(IPasswordHasher<ApiClient> hasher) => _hasher = hasher;

    public string GenerarClientId() => $"fb_live_{Convert.ToHexString(RandomNumberGenerator.GetBytes(12)).ToLowerInvariant()}";
    public string GenerarClientSecret() => $"fb_secret_{Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant()}";
    public string CrearHash(ApiClient apiClient, string secret) => _hasher.HashPassword(apiClient, secret);
    public bool Verificar(ApiClient apiClient, string secret)
        => _hasher.VerifyHashedPassword(apiClient, apiClient.ClientSecretHash, secret)
            is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
}
