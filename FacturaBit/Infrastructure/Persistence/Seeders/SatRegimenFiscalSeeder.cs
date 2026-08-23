using FacturaBit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FacturaBit.Infrastructure.Persistence.Seeders;

public static class SatRegimenFiscalSeeder
{
    public static void SeedSatRegimenFiscal(this ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2026, 8, 21, 20, 21, 33, DateTimeKind.Utc);
        modelBuilder.Entity<SatRegimenFiscal>().HasData(
                new SatRegimenFiscal { Id = 1, CveRegimen = "601", RegimenFiscal= "GENERAL DE LEY PERSONAS MORALES", Guid = "3a1c1e08-a225-4e0c-8cba-cecd1a7f4612", CreatedAt = seedDate, UpdatedAt = seedDate },
                new SatRegimenFiscal { Id = 2, CveRegimen = "603", RegimenFiscal = "PERSONAS MORALES CON FINES NO LUCRATIVOS", Guid = "a04a8875-dcc4-4644-b1ed-e3daf7748c1b", CreatedAt = seedDate, UpdatedAt = seedDate },
                new SatRegimenFiscal { Id = 3, CveRegimen = "605", RegimenFiscal = "SUELDOS Y SALARIOS E INGRESOS ASIMILADOS A SALARIOS", Guid = "a1c2fe58-bf50-408b-a1ce-4260f52a301a", CreatedAt = seedDate, UpdatedAt = seedDate },
                new SatRegimenFiscal { Id = 4, CveRegimen = "606", RegimenFiscal = "ARRENDAMIENTO", Guid = "141264d9-200b-488f-aec7-3610dbef91ae", CreatedAt = seedDate, UpdatedAt = seedDate },
                new SatRegimenFiscal { Id = 5, CveRegimen = "608", RegimenFiscal = "DEMÁS INGRESOS", Guid = "6ec15ed1-223e-4d85-8dc3-71a8cddd45f7", CreatedAt = seedDate, UpdatedAt = seedDate },
                new SatRegimenFiscal { Id = 6, CveRegimen = "609", RegimenFiscal = "CONSOLIDACIÓN", Guid = "f96ac295-f6c8-4a51-a0e3-d47effa0d5c6", CreatedAt = seedDate, UpdatedAt = seedDate },
                new SatRegimenFiscal { Id = 7, CveRegimen = "610", RegimenFiscal = "RESIDENTES EN EL EXTRANJERO SIN ESTABLECIMIENTO PERMANENTE EN MÉXICO", Guid = "322cb4dc-1265-4fd1-9d43-c0a508d9fc38", CreatedAt = seedDate, UpdatedAt = seedDate },
                new SatRegimenFiscal { Id = 8, CveRegimen = "611", RegimenFiscal = "INGRESOS POR DIVIDENDOS (SOCIOS Y ACCIONISTAS)", Guid = "b66af89c-a507-4242-a682-1732ae023d90", CreatedAt = seedDate, UpdatedAt = seedDate },
                new SatRegimenFiscal { Id = 9, CveRegimen = "612", RegimenFiscal = "PERSONAS FÍSICAS CON ACTIVIDADES EMPRESARIALES Y PROFESIONALES", Guid = "f5039019-d505-4b74-8802-0b8acac233e2", CreatedAt = seedDate, UpdatedAt = seedDate },
                new SatRegimenFiscal { Id = 10, CveRegimen = "614", RegimenFiscal = "INGRESOS POR INTERESES", Guid = "572f39b2-c4e0-484b-8acb-3e4b2276f13a", CreatedAt = seedDate, UpdatedAt = seedDate },
                new SatRegimenFiscal { Id = 11, CveRegimen = "616", RegimenFiscal = "SIN OBLIGACIONES FISCALES", Guid = "7946b1ad-eedb-4b75-bb7e-588ef0e5ef08", CreatedAt = seedDate, UpdatedAt = seedDate },
                new SatRegimenFiscal { Id = 12, CveRegimen = "620", RegimenFiscal = "SOCIEDADES COOPERATIVAS DE PRODUCCIÓN QUE OPTAN POR DIFERIR SUS INGRESOS", Guid = "ba166528-74db-4bd7-8812-3055d024f840", CreatedAt = seedDate, UpdatedAt = seedDate },
                new SatRegimenFiscal { Id = 13, CveRegimen = "621", RegimenFiscal = "INCORPORACIÓN FISCAL", Guid = "632a6438-ef84-4921-9d03-c03b4f2aba76", CreatedAt = seedDate, UpdatedAt = seedDate },
                new SatRegimenFiscal { Id = 14, CveRegimen = "622", RegimenFiscal = "ACTIVIDADES AGRÍCOLAS, GANADERAS, SILVÍCOLAS Y PESQUERAS", Guid = "d4679262-60a7-4fb0-bf36-13dc4a283474", CreatedAt = seedDate, UpdatedAt = seedDate },
                new SatRegimenFiscal { Id = 15, CveRegimen = "623", RegimenFiscal = "OPCIONAL PARA GRUPOS DE SOCIEDADES", Guid = "2b3fcec3-7f93-4ed7-8344-37b86932ea35", CreatedAt = seedDate, UpdatedAt = seedDate },
                new SatRegimenFiscal { Id = 16, CveRegimen = "624", RegimenFiscal = "COORDINADOS", Guid = "e8a4c6b3-6e3f-4558-b283-8304cc925fd9", CreatedAt = seedDate, UpdatedAt = seedDate },
                new SatRegimenFiscal { Id = 17, CveRegimen = "628", RegimenFiscal = "HIDROCARBUROS", Guid = "9620335d-e312-47cb-af7b-96b50deded1f", CreatedAt = seedDate, UpdatedAt = seedDate },
                new SatRegimenFiscal { Id = 18, CveRegimen = "607", RegimenFiscal = "RÉGIMEN DE ENAJENACIÓN O ADQUISICIÓN DE BIENES", Guid = "d1ce0849-d62d-4cad-b2d4-3ff07d818e0f", CreatedAt = seedDate, UpdatedAt = seedDate },
                new SatRegimenFiscal { Id = 19, CveRegimen = "629", RegimenFiscal = "DE LOS REGÍMENES FISCALES PREFERENTES Y DE LAS EMPRESAS MULTINACIONALES", Guid = "bf4246d9-9749-4114-aea5-8bf35847df1c", CreatedAt = seedDate, UpdatedAt = seedDate },
                new SatRegimenFiscal { Id = 20, CveRegimen = "630", RegimenFiscal = "ENAJENACIÓN DE ACCIONES EN BOLSA DE VALORES", Guid = "5bb1e920-0e59-4328-acca-cbc8f67d90ae", CreatedAt = seedDate, UpdatedAt = seedDate },
                new SatRegimenFiscal { Id = 21, CveRegimen = "615", RegimenFiscal = "RÉGIMEN DE LOS INGRESOS POR OBTENCIÓN DE PREMIOS", Guid = "248e523e-8685-48e8-9dac-d898e92044ec", CreatedAt = seedDate, UpdatedAt = seedDate },
                new SatRegimenFiscal { Id = 22, CveRegimen = "626", RegimenFiscal = "RÉGIMEN SIMPLIFICADO DE CONFIANZA", Guid = "6eff09ee-49bc-49cf-bb3b-f80b1d646613", CreatedAt = seedDate, UpdatedAt = seedDate }
        );
    }
}
