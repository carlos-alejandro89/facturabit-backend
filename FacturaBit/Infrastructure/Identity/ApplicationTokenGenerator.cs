using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FacturaBit.Infrastructure.Identity;

public sealed class ApplicationTokenGenerator : IApplicationTokenGenerator
{
    private readonly IConfiguration _configuration;
    public ApplicationTokenGenerator(IConfiguration configuration) => _configuration = configuration;

    public ClientAccessTokenDto Generar(ApiClient apiClient, string scope)
    {
        IConfigurationSection settings = _configuration.GetRequiredSection("JwtSettings");
        string key = settings["Key"] ?? throw new InvalidOperationException("JwtSettings:Key no está configurado.");
        string issuer = settings["Issuer"] ?? throw new InvalidOperationException("JwtSettings:Issuer no está configurado.");
        string audience = settings["Audience"] ?? throw new InvalidOperationException("JwtSettings:Audience no está configurado.");
        int expirationMinutes = settings.GetValue("ClientCredentialsExpirationMinutes", 60);
        DateTime now = DateTime.UtcNow;
        DateTime expiresAt = now.AddMinutes(expirationMinutes);
        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, apiClient.Guid),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(now).ToString(), ClaimValueTypes.Integer64),
            new("client_id", apiClient.ClientId),
            new("auth_type", "client_credentials"),
            new("red_comercial_id", apiClient.RedComercialId.ToString()),
            new("red_comercial_guid", apiClient.RedComercial.Guid),
            new("scope", scope)
        ];
        var token = new JwtSecurityToken(issuer, audience, claims, now, expiresAt,
            new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256));
        return new ClientAccessTokenDto(new JwtSecurityTokenHandler().WriteToken(token), expirationMinutes * 60, scope);
    }
}
