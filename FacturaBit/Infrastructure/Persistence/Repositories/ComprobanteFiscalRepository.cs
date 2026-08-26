using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FacturaBit.Infrastructure.Persistence.Repositories;

public sealed class ComprobanteFiscalRepository : IComprobanteFiscalRepository
{
    private const int EstatusVigenteId = 2;
    private const int EstatusErrorId = 8;
    private readonly AppDBContext _dbContext;

    public ComprobanteFiscalRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(IReadOnlyCollection<ComprobanteFiscal> Items, int Total)> ObtenerPorRedComercialAsync(
        string redComercialGuid,
        string? emisorGuid,
        string? busqueda,
        string? estatus,
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken)
    {
        IQueryable<ComprobanteFiscal> query = _dbContext.Set<ComprobanteFiscal>()
            .AsNoTracking()
            .Include(item => item.EntidadFiscal)
            .Include(item => item.EstatusCfdi)
            .Where(item => item.EntidadFiscal.RedesComerciales
                .Any(relation => relation.RedComercial.Guid == redComercialGuid));

        if (!string.IsNullOrWhiteSpace(emisorGuid))
            query = query.Where(item => item.EntidadFiscal.Guid == emisorGuid);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            string term = busqueda.Trim().ToUpper();
            query = query.Where(item =>
                item.Folio.ToUpper().Contains(term) ||
                (item.SerieCfdi != null && item.SerieCfdi.ToUpper().Contains(term)) ||
                item.ReceptorNombre.ToUpper().Contains(term) ||
                item.ReceptorRfc.ToUpper().Contains(term) ||
                item.EntidadFiscal.Rfc.ToUpper().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(estatus))
        {
            string status = estatus.Trim().ToUpper();
            query = status == "EN PROCESO"
                ? query.Where(item => item.EstatusCfdi.NombreEstatus == "EN PROCESO" ||
                                      item.EstatusCfdi.NombreEstatus == "SOLICITUD TIMBRADO")
                : query.Where(item => item.EstatusCfdi.NombreEstatus.ToUpper() == status);
        }

        int total = await query.CountAsync(cancellationToken);
        ComprobanteFiscal[] items = await query
            .OrderByDescending(item => item.CreatedAt)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToArrayAsync(cancellationToken);

        return (items, total);
    }

    public Task<ComprobanteFiscal?> ObtenerPorGuidYRedComercialAsync(
        string guid,
        string redComercialGuid,
        CancellationToken cancellationToken)
    {
        return _dbContext.Set<ComprobanteFiscal>()
            .AsNoTracking()
            .Include(item => item.EntidadFiscal)
            .Include(item => item.EstatusCfdi)
            .SingleOrDefaultAsync(item => item.Guid == guid &&
                item.EntidadFiscal.RedesComerciales.Any(relation =>
                    relation.RedComercial.Guid == redComercialGuid), cancellationToken);
    }

    public async Task InsertarAsync(ComprobanteFiscal comprobante, CancellationToken cancellationToken)
    {
        _dbContext.Add(comprobante);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task MarcarErrorAsync(int id, string mensaje, CancellationToken cancellationToken)
    {
        await _dbContext.Set<ComprobanteFiscal>()
            .Where(item => item.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(item => item.EstatusCfdiId, EstatusErrorId)
                .SetProperty(item => item.Success, false)
                .SetProperty(item => item.Mensaje, mensaje)
                .SetProperty(item => item.UpdatedAt, DateTime.UtcNow),
                cancellationToken);
    }

    public async Task MarcarVigenteAsync(
        int id,
        FacturacionResultDto resultado,
        string mensaje,
        string? pathXml,
        CancellationToken cancellationToken)
    {
        DateTime fechaTimbrado = DateTime.TryParse(resultado.FechaTimbrado, out DateTime parsedDate)
            ? parsedDate.ToUniversalTime()
            : DateTime.UtcNow;

        await _dbContext.Set<ComprobanteFiscal>()
            .Where(item => item.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(item => item.EstatusCfdiId, EstatusVigenteId)
                .SetProperty(item => item.Success, true)
                .SetProperty(item => item.Mensaje, mensaje)
                .SetProperty(item => item.NoCertificadoSat, resultado.NoCertificadoSat)
                .SetProperty(item => item.NoCertificadoEmisor, resultado.NoCertificadoEmisor)
                .SetProperty(item => item.FolioFiscal, resultado.Uuid)
                .SetProperty(item => item.SelloSat, resultado.SelloSat)
                .SetProperty(item => item.SelloEmisor, resultado.SelloEmisor)
                .SetProperty(item => item.FechaTimbrado, fechaTimbrado)
                .SetProperty(item => item.CadenaOriginalSat, resultado.CadenaOriginalSat)
                .SetProperty(item => item.PathXmlCfdi, pathXml)
                .SetProperty(item => item.UpdatedAt, DateTime.UtcNow),
                cancellationToken);
    }
}
