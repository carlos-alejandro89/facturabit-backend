using Microsoft.AspNetCore.Mvc;

namespace FacturaBit.Api.Contracts.Requests;

public sealed class CargaCertificadoRequest
{
    [FromForm(Name = "certificado")]
    public IFormFile? Certificado { get; init; }

    [FromForm(Name = "llave")]
    public IFormFile? Llave { get; init; }

    [FromForm(Name = "password")]
    public string? Password { get; init; }
}
