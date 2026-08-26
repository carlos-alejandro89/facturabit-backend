namespace FacturaBit.Application.DTOs;

public sealed class LoginResponseDto
{
    public required string Token { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public required string AccountGuid { get; init; }
    public required RedComercialLoginDto RedComercial { get; init; }
    public required string Email { get; init; }
    public required string FullName { get; init; }
    public required string Rol { get; init; }
    public bool RequiereCambioPassword { get; init; }
}

public sealed class RedComercialLoginDto
{
    public required string Guid { get; init; }
    public required string NombreRedComercial { get; init; }
}
