using System.Text.Json;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Features;

public sealed class ComprobanteFiscalService : IComprobanteFiscalService
{
    private readonly IComprobanteFiscalRepository _repository;
    private readonly ICertificateStorage _storage;
    private readonly IDocumentPdfService _pdfService;

    public ComprobanteFiscalService(
        IComprobanteFiscalRepository repository,
        ICertificateStorage storage,
        IDocumentPdfService pdfService)
    {
        _repository = repository;
        _storage = storage;
        _pdfService = pdfService;
    }

    public async Task<ComprobanteArchivoDto?> ObtenerPdfAsync(
        string guid,
        string redComercialGuid,
        CancellationToken cancellationToken)
    {
        ComprobanteFiscal? entity = await _repository.ObtenerPorGuidYRedComercialAsync(
            guid, redComercialGuid, cancellationToken);
        if (entity is null || string.IsNullOrWhiteSpace(entity.PathXmlCfdi)) return null;
        string? xml = await _storage.LeerCfdiAsync(entity.PathXmlCfdi, cancellationToken);
        if (xml is null) return null;
        return new ComprobanteArchivoDto
        {
            NombreArchivo = Path.ChangeExtension(Path.GetFileName(entity.PathXmlCfdi), ".pdf"),
            Contenido = _pdfService.GenerarCfdi(xml),
            ContentType = "application/pdf"
        };
    }

    public async Task<ComprobanteArchivoDto?> ObtenerAcusePdfAsync(
        string guid,
        string redComercialGuid,
        CancellationToken cancellationToken)
    {
        ComprobanteFiscal? entity = await _repository.ObtenerPorGuidYRedComercialAsync(
            guid, redComercialGuid, cancellationToken);
        if (entity is null || string.IsNullOrWhiteSpace(entity.PathXmlCancelacion)) return null;
        byte[]? pdf = await _storage.LeerPdfAsync(entity.PathXmlCancelacion, cancellationToken);
        if (pdf is null)
        {
            string? xml = await _storage.LeerCfdiAsync(entity.PathXmlCancelacion, cancellationToken);
            if (xml is null) return null;
            pdf = _pdfService.GenerarAcuseCancelacion(xml);
        }
        return new ComprobanteArchivoDto
        {
            NombreArchivo = Path.ChangeExtension(Path.GetFileName(entity.PathXmlCancelacion), ".pdf"),
            Contenido = pdf,
            ContentType = "application/pdf"
        };
    }

    public async Task<ComprobanteXmlDto?> ObtenerXmlAsync(
        string guid,
        string redComercialGuid,
        CancellationToken cancellationToken)
    {
        ComprobanteFiscal? entity = await _repository.ObtenerPorGuidYRedComercialAsync(
            guid, redComercialGuid, cancellationToken);
        if (entity is null || string.IsNullOrWhiteSpace(entity.PathXmlCfdi)) return null;

        string? content = await _storage.LeerCfdiAsync(entity.PathXmlCfdi, cancellationToken);
        if (content is null) return null;
        return new ComprobanteXmlDto
        {
            ComprobanteGuid = entity.Guid,
            NombreArchivo = Path.GetFileName(entity.PathXmlCfdi),
            Contenido = content
        };
    }

    public async Task<ComprobantesPageDto> ObtenerAsync(
        string redComercialGuid,
        string? emisorGuid,
        string? busqueda,
        string? estatus,
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken)
    {
        pagina = Math.Max(1, pagina);
        tamanoPagina = Math.Clamp(tamanoPagina, 1, 100);

        (IReadOnlyCollection<ComprobanteFiscal> items, int total) = await _repository.ObtenerPorRedComercialAsync(
            redComercialGuid, emisorGuid, busqueda, estatus, pagina, tamanoPagina, cancellationToken);

        return new ComprobantesPageDto
        {
            Items = items.Select(Mapear).ToArray(),
            Pagina = pagina,
            TamanoPagina = tamanoPagina,
            TotalRegistros = total,
            TotalPaginas = total == 0 ? 0 : (int)Math.Ceiling(total / (double)tamanoPagina)
        };
    }

    private static ComprobanteFiscalDto Mapear(ComprobanteFiscal entity) => new()
    {
        Guid = entity.Guid,
        Folio = entity.Folio,
        Serie = entity.SerieCfdi,
        ReceptorRfc = entity.ReceptorRfc,
        ReceptorNombre = entity.ReceptorNombre,
        Total = ObtenerTotal(entity.JsonRequest),
        Tipo = entity.TipoRequest,
        Estatus = entity.EstatusCfdi.NombreEstatus,
        Mensaje = entity.Mensaje,
        FolioFiscal = string.IsNullOrWhiteSpace(entity.FolioFiscal) ? null : entity.FolioFiscal,
        Fecha = entity.FechaTimbrado > DateTime.UnixEpoch ? entity.FechaTimbrado : entity.CreatedAt,
        PathXmlCfdi = entity.PathXmlCfdi,
        TieneAcuse = !string.IsNullOrWhiteSpace(entity.PathXmlCancelacion),
        Emisor = new ComprobanteEmisorDto
        {
            Guid = entity.EntidadFiscal.Guid,
            Rfc = entity.EntidadFiscal.Rfc,
            RazonSocial = entity.EntidadFiscal.RazonSocial,
            NombreComercial = entity.EntidadFiscal.NombreComercial
        }
    };

    private static decimal ObtenerTotal(JsonDocument request)
    {
        if (!request.RootElement.TryGetProperty("invoice", out JsonElement invoice)) return 0;
        if (!invoice.TryGetProperty("Total", out JsonElement total) &&
            !invoice.TryGetProperty("total", out total)) return 0;
        return total.TryGetDecimal(out decimal value) ? value : 0;
    }
}
