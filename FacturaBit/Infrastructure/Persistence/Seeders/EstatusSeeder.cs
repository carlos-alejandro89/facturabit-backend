using FacturaBit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FacturaBit.Infrastructure.Persistence.Seeders;

public static class EstatusSeeder
{
    public static void SeedEstatus(this ModelBuilder modelBuilder)
    {
       var seedDate = new DateTime(2026, 8, 21, 20, 21, 33, DateTimeKind.Utc);

        modelBuilder.Entity<Estatus>().HasData(
            new Estatus { Id = 1, NombreEstatus = "SOLICITUD TIMBRADO", DescripcionEstatus = "Se recibe solicitud de timbrado JSON.", CreatedAt = seedDate, UpdatedAt = seedDate, Guid = "f0e47c96-4708-46b6-a914-1830e2cfd252" },
            new Estatus { Id = 2, NombreEstatus = "VIGENTE", DescripcionEstatus = "Comprobante timbrado y activo fiscalmente ante el SAT.", CreatedAt = seedDate, UpdatedAt = seedDate, Guid = "582d00c4-cd5b-48c0-8c14-0735db2b8866" },
            new Estatus { Id = 3, NombreEstatus = "CANCELADO", DescripcionEstatus = "Comprobante anulado formalmente ante el SAT.", CreatedAt = seedDate, UpdatedAt = seedDate, Guid = "a38f12d4-1e28-4b9a-b182-9382f102a4bc" },
            new Estatus { Id = 4, NombreEstatus = "SIN ACEPTABILIDAD", DescripcionEstatus = "Cancela de inmediato sin pedir permiso al receptor (por monto, tipo de factura o tiempo transcurrido).", CreatedAt = seedDate, UpdatedAt = seedDate, Guid = "b49e23e5-2f39-4c0b-a293-0493a213b5cd" },
            new Estatus { Id = 5, NombreEstatus = "EN PROCESO", DescripcionEstatus = "Solicitada. Se envió la notificación al Buzón Tributario del receptor y está en espera de su respuesta.", CreatedAt = seedDate, UpdatedAt = seedDate, Guid = "c50f34f6-3a40-4d1c-b304-1504b324c6de" },
            new Estatus { Id = 6, NombreEstatus = "ACEPTADO", DescripcionEstatus = "El receptor aprobó la cancelación desde su Buzón Tributario.", CreatedAt = seedDate, UpdatedAt = seedDate, Guid = "d61a45a7-4b51-4e2d-c415-2615c435d7ef" },
            new Estatus { Id = 7, NombreEstatus = "RECHAZADO", DescripcionEstatus = "El receptor negó la cancelación. La factura se mantiene Vigente.", CreatedAt = seedDate, UpdatedAt = seedDate, Guid = "e72b56b8-5c62-4f3e-d526-3726d546e800" },
            new Estatus { Id = 8, NombreEstatus = "ERROR", DescripcionEstatus = "Ocurrió un error al generar, validar o timbrar el XML.", CreatedAt = seedDate, UpdatedAt = seedDate, Guid = "f83c67c9-6d73-404f-e637-4837e657f911" },
            new Estatus { Id = 9, NombreEstatus = "COMPLETADA", DescripcionEstatus = "La operación se completó satisfactoriamente.", CreatedAt = seedDate, UpdatedAt = seedDate, Guid = "a9c3ee31-6bb3-4e4c-8d12-80a455bd635f" },
            new Estatus { Id = 10, NombreEstatus = "REEMBOLSADA", DescripcionEstatus = "El importe de la operación fue reembolsado al cliente.", CreatedAt = seedDate, UpdatedAt = seedDate, Guid = "a00daf3a-eaed-48c9-bc4d-247013e48980" }
        );
    }
}
