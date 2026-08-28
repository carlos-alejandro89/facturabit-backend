namespace FacturaBit.Application.DTOs;

public sealed record ApiClientDto(
    string Guid,
    string Nombre,
    string ClientId,
    IReadOnlyCollection<string> Scopes,
    bool Activo,
    DateTime? ExpiresAt,
    DateTime? LastUsedAt,
    DateTime CreatedAt);

public sealed record ApiClientCreatedDto(
    string Guid,
    string Nombre,
    string ClientId,
    string ClientSecret,
    IReadOnlyCollection<string> Scopes,
    DateTime CreatedAt);

public sealed record ClientAccessTokenDto(
    string AccessToken,
    int ExpiresIn,
    string Scope);
