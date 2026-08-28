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
    private readonly IApiClientService _apiClientService;

    public AuthenticationController(IAuthenticationService authenticationService, IApiClientService apiClientService)
    {
        _authenticationService = authenticationService;
        _apiClientService = apiClientService;
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
