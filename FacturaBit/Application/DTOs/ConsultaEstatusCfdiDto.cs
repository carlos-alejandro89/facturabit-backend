namespace FacturaBit.Application.DTOs;

public sealed class ConsultaEstatusCfdiResultDto
{
    public required string Uuid { get; init; }
    public required string Estado { get; init; }
    public required string CodigoEstatus { get; init; }
    public string? EsCancelable { get; init; }
    public string? EstatusCancelacion { get; init; }
    public string? ValidacionEfos { get; init; }
    public DateTime ConsultadoEn { get; init; }
}

public sealed class SatConsultaEstatusRequestDto
{
    public required string RfcEmisor { get; init; }
    public required string RfcReceptor { get; init; }
    public decimal Total { get; init; }
    public required string Uuid { get; init; }
    public required string UltimosOchoSello { get; init; }
}

public sealed class SatConsultaEstatusResultDto
{
    public required string Estado { get; init; }
    public required string CodigoEstatus { get; init; }
    public string? EsCancelable { get; init; }
    public string? EstatusCancelacion { get; init; }
    public string? ValidacionEfos { get; init; }
}
