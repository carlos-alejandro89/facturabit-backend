using FacturaBit.Api.Contracts.Requests;
using FacturaBit.Api.Contracts.Responses;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FacturaBit.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IApiClientService _apiClientService;
    private readonly IPasswordRecoveryService _passwordRecoveryService;

    public AuthenticationController(
        IAuthenticationService authenticationService,
        IApiClientService apiClientService,
        IPasswordRecoveryService passwordRecoveryService)
    {
        _authenticationService = authenticationService;
        _apiClientService = apiClientService;
        _passwordRecoveryService = passwordRecoveryService;
    }

    [AllowAnonymous]
    [EnableRateLimiting("PasswordRecovery")]
    [HttpPost("password-recovery/request")]
    public async Task<ActionResult<ApiResponse<object>>> RequestPasswordRecovery(
        RequestPasswordRecoveryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _passwordRecoveryService.SolicitarAsync(request.Email, cancellationToken);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Mensaje = "Si el correo está registrado, recibirás un código de recuperación en unos minutos.",
                HttpCode = StatusCodes.Status200OK
            });
        }
        catch (FacturaBit.Application.Exceptions.EmailDeliveryException exception)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new ApiResponse<object>
            {
                Success = false,
                Mensaje = exception.Message,
                HttpCode = StatusCodes.Status503ServiceUnavailable
            });
        }
    }

    [AllowAnonymous]
    [EnableRateLimiting("PasswordRecovery")]
    [HttpPost("password-recovery/verify")]
    public async Task<ActionResult<ApiResponse<object>>> VerifyPasswordRecoveryCode(
        VerifyPasswordRecoveryCodeRequest request,
        CancellationToken cancellationToken)
    {
        bool valid = await _passwordRecoveryService.VerificarAsync(
            request.Email,
            request.Code,
            cancellationToken);
        return valid
            ? Ok(new ApiResponse<object>
            {
                Success = true,
                Mensaje = "Código verificado correctamente.",
                HttpCode = StatusCodes.Status200OK
            })
            : BadRequest(new ApiResponse<object>
            {
                Success = false,
                Mensaje = "El código es inválido, expiró o superó el número de intentos permitidos.",
                HttpCode = StatusCodes.Status400BadRequest
            });
    }

    [AllowAnonymous]
    [EnableRateLimiting("PasswordRecovery")]
    [HttpPost("password-recovery/reset")]
    public async Task<ActionResult<ApiResponse<object>>> ResetPassword(
        ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _passwordRecoveryService.RestablecerAsync(
                request.Email,
                request.Code,
                request.NewPassword,
                cancellationToken);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Mensaje = "Tu contraseña fue actualizada correctamente.",
                HttpCode = StatusCodes.Status200OK
            });
        }
        catch (FacturaBit.Application.Exceptions.PasswordRecoveryException exception)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Mensaje = exception.Message,
                HttpCode = StatusCodes.Status400BadRequest
            });
        }
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

    [AllowAnonymous]
    [HttpPost("client-token")]
    [HttpPost("/oauth/token")]
    [Consumes("application/x-www-form-urlencoded")]
    [ProducesResponseType(typeof(OAuthTokenResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ClientToken([FromForm] ClientCredentialsTokenRequest request, CancellationToken cancellationToken)
    {
        if (!string.Equals(request.GrantType, "client_credentials", StringComparison.Ordinal))
            return BadRequest(new { error = "unsupported_grant_type", error_description = "grant_type debe ser client_credentials." });

        (string? clientId, string? clientSecret) = ObtenerCredencialesBasicas();
        clientId ??= request.ClientId;
        clientSecret ??= request.ClientSecret;
        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            return InvalidClient();

        try
        {
            ClientAccessTokenDto? token = await _apiClientService.AutenticarAsync(clientId, clientSecret, request.Scope, cancellationToken);
            if (token is null) return InvalidClient();
            Response.Headers.CacheControl = "no-store";
            Response.Headers.Pragma = "no-cache";
            return Ok(new OAuthTokenResponse(token.AccessToken, "Bearer", token.ExpiresIn, token.Scope));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = "invalid_scope", error_description = exception.Message });
        }
    }

    private IActionResult InvalidClient()
    {
        Response.Headers.WWWAuthenticate = "Basic realm=\"FacturaBit OAuth\"";
        return Unauthorized(new { error = "invalid_client", error_description = "Las credenciales de aplicación no son válidas." });
    }

    private (string? ClientId, string? ClientSecret) ObtenerCredencialesBasicas()
    {
        string authorization = Request.Headers.Authorization.ToString();
        if (!authorization.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase)) return (null, null);
        try
        {
            string decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(authorization[6..].Trim()));
            int separator = decoded.IndexOf(':');
            return separator > 0 ? (decoded[..separator], decoded[(separator + 1)..]) : (null, null);
        }
        catch (FormatException) { return (null, null); }
    }
}
