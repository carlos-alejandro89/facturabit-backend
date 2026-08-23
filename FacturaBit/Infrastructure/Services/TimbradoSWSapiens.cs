using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Application.Exceptions;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace FacturaBit.Infrastructure.Services;

public sealed class TimbradoSWSapiens : ITimbradoService
{
    private readonly IConfiguration _configuration;

    public TimbradoSWSapiens(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<TimbradoResultDto> TimbrarAsync(string xml, CancellationToken cancellationToken)
    {
        RestClient client = CrearCliente();
        string token = await ObtenerTokenAsync(client, cancellationToken);

        RestRequest request = new("/cfdi33/stamp/json/v4", Method.Post);
        request.AddHeader("Authorization", $"Bearer {token}");
        request.AddJsonBody(new { data = xml });

        JObject response = await EjecutarAsync(client, request, cancellationToken);
        bool success = string.Equals(response.Value<string>("status"), "success", StringComparison.OrdinalIgnoreCase);
        JToken? data = response["data"];

        if (!success || data is null)
        {
            return new TimbradoResultDto
            {
                Success = false,
                Mensaje = ObtenerMensaje(response)
            };
        }

        return new TimbradoResultDto
        {
            Success = true,
            Cfdi = data.Value<string>("cfdi"),
            NoCertificadoSat = data.Value<string>("noCertificadoSAT"),
            NoCertificadoEmisor = data.Value<string>("noCertificadoCFDI"),
            Uuid = data.Value<string>("uuid"),
            SelloSat = data.Value<string>("selloSAT"),
            SelloEmisor = data.Value<string>("selloCFDI"),
            FechaTimbrado = data.Value<string>("fechaTimbrado"),
            CadenaOriginalSat = data.Value<string>("cadenaOriginalSAT")
        };
    }

    public async Task<ProveedorCancelacionResultDto> CancelarAsync(
        CancelacionDto cancelacion,
        CancellationToken cancellationToken)
    {
        RestClient client = CrearCliente();
        string token = await ObtenerTokenAsync(client, cancellationToken);

        RestRequest request = new("/cfdi33/cancel/csd", Method.Post);
        request.AddHeader("Authorization", $"Bearer {token}");
        request.AddJsonBody(new
        {
            uuid = cancelacion.UUID,
            rfc = cancelacion.RFC,
            password = cancelacion.Password,
            motivo = cancelacion.Motivo,
            b64Cer = cancelacion.CerB64,
            b64Key = cancelacion.KeyB64
        });

        JObject response = await EjecutarAsync(client, request, cancellationToken);
        bool success = string.Equals(response.Value<string>("status"), "success", StringComparison.OrdinalIgnoreCase);

        return new ProveedorCancelacionResultDto
        {
            Success = success,
            Mensaje = success ? null : ObtenerMensaje(response),
            Detalle = response.Value<string>("messageDetail"),
            Acuse = response["data"]?.Value<string>("acuse")
        };
    }

    private RestClient CrearCliente()
    {
        string url = ObtenerConfiguracion("TimbradoSW:url");
        return new RestClient(new RestClientOptions(url)
        {
            Timeout = TimeSpan.FromSeconds(60)
        });
    }

    private async Task<string> ObtenerTokenAsync(RestClient client, CancellationToken cancellationToken)
    {
        RestRequest request = new("/security/authenticate", Method.Post);
        request.AddHeader("user", ObtenerConfiguracion("TimbradoSW:usuario"));
        request.AddHeader("password", ObtenerConfiguracion("TimbradoSW:password"));

        JObject response = await EjecutarAsync(client, request, cancellationToken);
        string? token = response["data"]?.Value<string>("token");
        return !string.IsNullOrWhiteSpace(token)
            ? token
            : throw new FacturacionException("El proveedor de timbrado no devolvió un token de acceso.");
    }

    private static async Task<JObject> EjecutarAsync(
        RestClient client,
        RestRequest request,
        CancellationToken cancellationToken)
    {
        RestResponse response = await client.ExecuteAsync(request, cancellationToken);
        if (string.IsNullOrWhiteSpace(response.Content))
        {
            throw new FacturacionException(
                $"El proveedor de timbrado no devolvió contenido. HTTP {(int)response.StatusCode}.");
        }

        try
        {
            return JObject.Parse(response.Content);
        }
        catch (Exception exception)
        {
            throw new FacturacionException(
                $"El proveedor de timbrado devolvió una respuesta inválida: {exception.Message}");
        }
    }

    private string ObtenerConfiguracion(string key)
    {
        return _configuration[key]
            ?? throw new FacturacionException($"Falta configurar '{key}'.");
    }

    private static string ObtenerMensaje(JObject response) =>
        response.Value<string>("message") ?? "El proveedor de timbrado rechazó la operación.";
}
