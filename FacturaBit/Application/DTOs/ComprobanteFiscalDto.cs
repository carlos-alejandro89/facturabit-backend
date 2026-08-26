namespace FacturaBit.Application.DTOs;

public sealed class ComprobanteFiscalDto
{
    public required string Guid { get; init; }
    public required string Folio { get; init; }
    public string? Serie { get; init; }
    public required string ReceptorRfc { get; init; }
    public required string ReceptorNombre { get; init; }
    public decimal Total { get; init; }
    public required string Tipo { get; init; }
    public required string Estatus { get; init; }
    public required string Mensaje { get; init; }
    public string? FolioFiscal { get; init; }
    public DateTime Fecha { get; init; }
    public string? PathXmlCfdi { get; init; }
    public required ComprobanteEmisorDto Emisor { get; init; }
}

public sealed class ComprobanteXmlDto
{
    public required string ComprobanteGuid { get; init; }
    public required string NombreArchivo { get; init; }
    public required string Contenido { get; init; }
}

public sealed class ComprobanteEmisorDto
{
    public required string Guid { get; init; }
    public required string Rfc { get; init; }
    public required string RazonSocial { get; init; }
    public string? NombreComercial { get; init; }
}

public sealed class ComprobantesPageDto
{
    public required IReadOnlyCollection<ComprobanteFiscalDto> Items { get; init; }
    public int Pagina { get; init; }
    public int TamanoPagina { get; init; }
    public int TotalRegistros { get; init; }
    public int TotalPaginas { get; init; }
}
