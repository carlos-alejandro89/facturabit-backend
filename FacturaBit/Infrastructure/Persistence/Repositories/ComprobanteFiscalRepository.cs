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
