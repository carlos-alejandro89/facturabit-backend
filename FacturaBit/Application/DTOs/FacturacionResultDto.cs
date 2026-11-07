namespace FacturaBit.Application.DTOs;

public sealed class FacturacionResultDto
{
    public required string NoCertificadoSat { get; init; }
    public required string NoCertificadoEmisor { get; init; }
    public required string Uuid { get; init; }
    public required string SelloSat { get; init; }
    public required string SelloEmisor { get; init; }
    public required string FechaTimbrado { get; init; }
    public required string CadenaOriginalSat { get; init; }
    public required string CfdiXmlBase64 { get; init; }
}

public sealed class CancelacionResultDto
{
    public string? AcuseBase64 { get; init; }
    public string? AcusePdfBase64 { get; init; }
    public bool Cancelado { get; init; }
    public string? EstatusSat { get; init; }
    public string? CodigoEstatusSat { get; init; }
    public string? EsCancelable { get; init; }
    public string? EstatusCancelacion { get; init; }
    public IReadOnlyDictionary<string, string> EstatusPorUuid { get; init; } =
        new Dictionary<string, string>();
}

public sealed class TimbradoResultDto
{
    public bool Success { get; init; }
    public string? Mensaje { get; init; }
    public string? Cfdi { get; init; }
    public string? NoCertificadoSat { get; init; }
    public string? NoCertificadoEmisor { get; init; }
    public string? Uuid { get; init; }
    public string? SelloSat { get; init; }
    public string? SelloEmisor { get; init; }
    public string? FechaTimbrado { get; init; }
    public string? CadenaOriginalSat { get; init; }
}

public sealed class ProveedorCancelacionResultDto
{
    public bool Success { get; init; }
    public string? Mensaje { get; init; }
    public string? Detalle { get; init; }
    public string? Acuse { get; init; }
    public string? StatusSat { get; init; }
    public string? StatusCodeSat { get; init; }
    public string? IsCancelable { get; init; }
    public string? StatusCancelation { get; init; }
    public IReadOnlyDictionary<string, string> UuidStatuses { get; init; } =
        new Dictionary<string, string>();
}
