using FacturaBit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FacturaBit.Infrastructure.Persistence.Seeders;

public static class SatMotivosCancelacionSeeder
{
    public static void SeedSatMotivosCancelacion(this ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2026, 8, 21, 20, 21, 33, DateTimeKind.Utc);

        modelBuilder.Entity<SatMotivosCancelacion>().HasData(
            new SatMotivosCancelacion
            {
                Id = 1,
                CveMotivo = "01",
                MotivoCancelacion = "COMPROBANTES EMITIDOS CON ERRORES CON RELACIÓN",
                RequiereFolioSustitucion = true,
                Guid = "2c7833b3-5570-4b97-b626-5d881e491a35",
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new SatMotivosCancelacion
            {
                Id = 2,
                CveMotivo = "02",
                MotivoCancelacion = "COMPROBANTES EMITIDOS CON ERRORES SIN RELACIÓN",
                RequiereFolioSustitucion = false,
                Guid = "aad0a49f-fb42-4bbb-9dc7-c8ad097d5ae0",
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new SatMotivosCancelacion
            {
                Id = 3,
                CveMotivo = "03",
                MotivoCancelacion = "NO SE LLEVÓ A CABO LA OPERACIÓN",
                RequiereFolioSustitucion = false,
                Guid = "09948acc-2002-4cbd-a16e-c6a127515a93",
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new SatMotivosCancelacion
            {
                Id = 4,
                CveMotivo = "04",
                MotivoCancelacion = "OPERACIÓN NOMINATIVA RELACIONADA EN UNA FACTURA GLOBAL",
                RequiereFolioSustitucion = false,
                Guid = "0d887f32-485c-4d10-92ce-0228a22d4006",
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            });
    }
}
