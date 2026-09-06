using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Application.Exceptions;
using Microsoft.Extensions.Options;

namespace FacturaBit.Infrastructure.Services;

public sealed class SatConsultaEstatusClient : ISatConsultaEstatusClient
{
    private readonly HttpClient _httpClient;
    private readonly SatConsultaEstatusOptions _options;

    public SatConsultaEstatusClient(
        HttpClient httpClient,
        IOptions<SatConsultaEstatusOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<SatConsultaEstatusResultDto> ConsultarAsync(
        SatConsultaEstatusRequestDto request,
        CancellationToken cancellationToken)
    {
        string endpoint = ObtenerEndpoint();
        string expresion = string.Create(
            CultureInfo.InvariantCulture,
            $"?re={request.RfcEmisor}&rr={request.RfcReceptor}&tt={request.Total:0.######}&id={request.Uuid}&fe={request.UltimosOchoSello}");
        string soapEnvelope = ConstruirSoapEnvelope(expresion);

        using HttpRequestMessage httpRequest = new(HttpMethod.Post, endpoint);
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
        httpRequest.Headers.TryAddWithoutValidation("SOAPAction", _options.SoapAction);
        httpRequest.Content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

        using HttpResponseMessage response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        string responseXml = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new FacturacionException(
                $"El servicio de consulta del SAT respondió con HTTP {(int)response.StatusCode}.");
        }

        return ParsearRespuesta(responseXml);
    }

    private string ObtenerEndpoint()
    {
        bool pruebas = string.Equals(_options.Ambiente, "Pruebas", StringComparison.OrdinalIgnoreCase);
        bool produccion = string.Equals(
            _options.Ambiente,
            "Produccion",
            StringComparison.OrdinalIgnoreCase);
        if (!pruebas && !produccion)
            throw new FacturacionException(
                "SatConsultaEstatus:Ambiente debe ser Pruebas o Produccion.");

        string endpoint = produccion ? _options.UrlProduccion : _options.UrlPruebas;
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out _))
            throw new FacturacionException("La URL del servicio de consulta de estatus SAT no es válida.");

        return endpoint;
    }

    private static string ConstruirSoapEnvelope(string expresion)
    {
        XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";
        XNamespace tempuri = "http://tempuri.org/";
        XDocument document = new(
            new XElement(soap + "Envelope",
                new XAttribute(XNamespace.Xmlns + "soapenv", soap),
                new XAttribute(XNamespace.Xmlns + "tem", tempuri),
                new XElement(soap + "Header"),
                new XElement(soap + "Body",
                    new XElement(tempuri + "Consulta",
                        new XElement(tempuri + "expresionImpresa", expresion)))));
        return document.ToString(SaveOptions.DisableFormatting);
    }

    private static SatConsultaEstatusResultDto ParsearRespuesta(string responseXml)
    {
        try
        {
            using StringReader textReader = new(responseXml);
            using XmlReader xmlReader = XmlReader.Create(textReader, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null
            });
            XDocument document = XDocument.Load(xmlReader);

            string? fault = Valor(document, "faultstring");
            if (!string.IsNullOrWhiteSpace(fault))
                throw new FacturacionException($"El SAT rechazó la consulta: {fault}");

            string codigoEstatus = Valor(document, "CodigoEstatus")
                ?? throw new FacturacionException("La respuesta del SAT no contiene CodigoEstatus.");
            string estado = Valor(document, "Estado")
                ?? throw new FacturacionException("La respuesta del SAT no contiene Estado.");

            return new SatConsultaEstatusResultDto
            {
                CodigoEstatus = codigoEstatus,
                Estado = estado,
                EsCancelable = Valor(document, "EsCancelable"),
                EstatusCancelacion = Valor(document, "EstatusCancelacion"),
                ValidacionEfos = Valor(document, "ValidacionEFOS")
            };
        }
        catch (FacturacionException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new FacturacionException(
                "No fue posible interpretar la respuesta del servicio de consulta del SAT.",
                exception);
        }
    }

    private static string? Valor(XDocument document, string localName)
    {
        string? value = document.Descendants()
            .FirstOrDefault(element => element.Name.LocalName == localName)
            ?.Value
            .Trim();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
}
