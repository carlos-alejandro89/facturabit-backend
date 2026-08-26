using FacturaBit.Api.Contracts.Requests;
using FacturaBit.Api.Contracts.Responses;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacturaBit.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        LoginResponseDto? login = await _authenticationService.LoginAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (login is null)
        {
            return Unauthorized(new ApiResponse<LoginResponseDto>
            {
                Success = false,
                Mensaje = "El correo electrónico o la contraseña son incorrectos.",
                HttpCode = StatusCodes.Status401Unauthorized
            });
        }

        return Ok(new ApiResponse<LoginResponseDto>
        {
            Success = true,
            Mensaje = login.RequiereCambioPassword
                ? "Inicio de sesión correcto. Debes establecer una contraseña nueva."
                : "Inicio de sesión correcto.",
            HttpCode = StatusCodes.Status200OK,
            Data = login
        });
    }
}
