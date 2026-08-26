using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FacturaBit.Infrastructure.Identity;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public GeneratedTokenDto Generar(Accounts account, bool requiereCambioPassword)
    {
        IConfigurationSection settings = _configuration.GetRequiredSection("JwtSettings");
        string keyValue = settings["Key"]
            ?? throw new InvalidOperationException("JwtSettings:Key no está configurado.");
        string issuer = settings["Issuer"]
            ?? throw new InvalidOperationException("JwtSettings:Issuer no está configurado.");
        string audience = settings["Audience"]
            ?? throw new InvalidOperationException("JwtSettings:Audience no está configurado.");
        int expirationMinutes = settings.GetValue("ExpirationMinutes", 480);
        DateTime expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, account.Guid),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, account.Email),
            new(ClaimTypes.NameIdentifier, account.Guid),
            new(ClaimTypes.Name, account.FullName),
            new(ClaimTypes.Email, account.Email),
            new(ClaimTypes.Role, account.Rol.Nombre),
            new("red_comercial_guid", account.RedComercial.Guid),
            new("requiere_cambio_password", requiereCambioPassword.ToString().ToLowerInvariant())
        ];

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyValue)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new GeneratedTokenDto(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt);
    }
}
