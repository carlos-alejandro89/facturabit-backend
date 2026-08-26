using System.ComponentModel.DataAnnotations;

namespace FacturaBit.Api.Contracts.Requests;

public sealed class LoginRequest
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    [MaxLength(254)]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MaxLength(200)]
    public string Password { get; init; } = string.Empty;
}
