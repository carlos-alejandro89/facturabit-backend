using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Features;

public sealed class SatMotivosCancelacionService : ISatMotivosCancelacionService
{
    private readonly ISatMotivosCancelacionRepository _repository;

    public SatMotivosCancelacionService(ISatMotivosCancelacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<SatMotivoCancelacionDto>> ObtenerTodosAsync(
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<SatMotivosCancelacion> motivos =
            await _repository.ObtenerTodosAsync(cancellationToken);

        return motivos.Select(motivo => new SatMotivoCancelacionDto
        {
            Id = motivo.Id,
            Guid = motivo.Guid,
            Clave = motivo.CveMotivo,
            Nombre = motivo.MotivoCancelacion,
            RequiereFolioSustitucion = motivo.RequiereFolioSustitucion
        }).ToArray();
    }
}
