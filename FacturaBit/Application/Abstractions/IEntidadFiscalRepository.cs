using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Abstractions;

public interface IEntidadFiscalRepository
{
    Task<IReadOnlyCollection<EntidadFiscal>> ObtenerTodosAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<EntidadFiscal>> ObtenerPorRedComercialGuidAsync(string redComercialGuid, CancellationToken cancellationToken);
    Task<EntidadFiscal?> ObtenerPorGuidYRedComercialGuidAsync(string guid, string redComercialGuid, bool tracking, CancellationToken cancellationToken);
    Task<RedComercial?> ObtenerRedComercialPorGuidAsync(string guid, CancellationToken cancellationToken);
    Task<bool> ExisteRfcEnOtraEntidadAsync(string rfc, string? entidadFiscalGuid, CancellationToken cancellationToken);
    Task<EntidadFiscal?> ObtenerPorGuidAsync(string guid, bool incluirRelaciones, CancellationToken cancellationToken);
    Task<EntidadFiscal?> ObtenerPorRfcAsync(string rfc, CancellationToken cancellationToken);
    Task<bool> ExisteEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> ExisteRedComercialAsync(string nombre, CancellationToken cancellationToken);
    Task<Rol?> ObtenerRolPorNombreAsync(string nombre, CancellationToken cancellationToken);
    Task<Suscripcion?> ObtenerSuscripcionPorGuidAsync(string guid, CancellationToken cancellationToken);
    Task<Estatus?> ObtenerEstatusPorIdYGuidAsync(int id, string guid, CancellationToken cancellationToken);
    Task<bool> ExisteRegimenFiscalAsync(int id, CancellationToken cancellationToken);
    Task<bool> DescontarTimbrePorRfcAsync(string rfc, CancellationToken cancellationToken);
    Task CrearRegistroInicialAsync(
        RedComercial redComercial,
        EntidadFiscal entidadFiscal,
        RedComercialEntidadFiscal relacion,
        Accounts account,
        OrdenCompraSuscripcion ordenCompra,
        CancellationToken cancellationToken);
    Task GuardarCambiosAsync(CancellationToken cancellationToken);
    Task AgregarEntidadFiscalAsync(EntidadFiscal entidadFiscal, RedComercialEntidadFiscal relacion, CancellationToken cancellationToken);
    Task EliminarAsync(EntidadFiscal entidadFiscal, CancellationToken cancellationToken);
}
