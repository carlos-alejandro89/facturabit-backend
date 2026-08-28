using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace FacturaBit.Api.Contracts.Requests;

public sealed class CreateApiClientRequest
{
    [Required, MaxLength(120)]
    public string Nombre { get; init; } = string.Empty;
    public string[] Scopes { get; init; } = ["cfdi.emit"];
}

public sealed class ClientCredentialsTokenRequest
{
    [FromForm(Name = "grant_type")]
    public string GrantType { get; init; } = string.Empty;
    [FromForm(Name = "client_id")]
    public string? ClientId { get; init; }
    [FromForm(Name = "client_secret")]
    public string? ClientSecret { get; init; }
    [FromForm(Name = "scope")]
    public string? Scope { get; init; }
}
