namespace FacturaBit.Infrastructure.Services;

public sealed class SatConsultaEstatusOptions
{
    public const string SectionName = "SatConsultaEstatus";

    public string Ambiente { get; init; } = "Pruebas";
    public string UrlPruebas { get; init; } = string.Empty;
    public string UrlProduccion { get; init; } = string.Empty;
    public string SoapAction { get; init; } = "http://tempuri.org/IConsultaCFDIService/Consulta";
}
