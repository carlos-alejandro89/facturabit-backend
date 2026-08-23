using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Abstractions;

public interface IEntidadFiscalRepository
{
    Task<IReadOnlyCollection<EntidadFiscal>> ObtenerTodosAsync(CancellationToken cancellationToken);
    Task<EntidadFiscal?> ObtenerPorGuidAsync(string guid, bool incluirRelaciones, CancellationToken cancellationToken);
    Task<EntidadFiscal?> ObtenerPorRfcAsync(string rfc, CancellationToken cancellationToken);
    Task<bool> ExisteEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> ExisteRedComercialAsync(string nombre, CancellationToken cancellationToken);
    Task<Rol?> ObtenerRolPorNombreAsync(string nombre, CancellationToken cancellationToken);
    Task<bool> ExisteRegimenFiscalAsync(int id, CancellationToken cancellationToken);
    Task<bool> DescontarTimbrePorRfcAsync(string rfc, CancellationToken cancellationToken);
    Task CrearRegistroInicialAsync(
        RedComercial redComercial,
        EntidadFiscal entidadFiscal,
        RedComercialEntidadFiscal relacion,
        Accounts account,
        CancellationToken cancellationToken);
    Task GuardarCambiosAsync(CancellationToken cancellationToken);
    Task EliminarAsync(EntidadFiscal entidadFiscal, CancellationToken cancellationToken);
}
