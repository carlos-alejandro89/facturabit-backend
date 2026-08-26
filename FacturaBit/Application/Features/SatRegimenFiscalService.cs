using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Features;

public sealed class SatRegimenFiscalService : ISatRegimenFiscalService
{
    private readonly ISatRegimenFiscalRepository _repository;

    public SatRegimenFiscalService(ISatRegimenFiscalRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<SatRegimenFiscalDto>> ObtenerTodosAsync(
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<SatRegimenFiscal> regimenes = await _repository.ObtenerTodosAsync(cancellationToken);
        return regimenes.Select(regimen => new SatRegimenFiscalDto
        {
            Id = regimen.Id,
            Guid = regimen.Guid,
            Clave = regimen.CveRegimen,
            Nombre = regimen.RegimenFiscal
        }).ToArray();
    }
}
