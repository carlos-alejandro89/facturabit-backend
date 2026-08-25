using FacturaBit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FacturaBit.Infrastructure.Persistence.Seeders;

public static class SuscripcionSeeder
{
    public static void SeedSuscripciones(this ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2026, 8, 21, 20, 21, 33, DateTimeKind.Utc);

        modelBuilder.Entity<Suscripcion>().HasData(
            new Suscripcion
            {
                Id = 1,
                Codigo = "FREE",
                NombreSuscripcion = "Prueba gratuita 7 días",
                Caracteristicas = new CaracteristicasSucripcion
                {
                    TimbresIncluidos = 50,
                    EmisoresIncluidos = 1,
                    Vigencia = 7,
                    UnidadVigencia = "Días",
                    SoportePrioritario = false,
                    RespaldoCfdi = false,
                    AccesoApi = false,
                    Recomendado = false
                },
                Precio = 0m,
                CreatedAt = seedDate,
                UpdatedAt = seedDate,
                Guid = "4ea467b7-58a0-47ad-8310-bf5575af103e"
            },
            new Suscripcion
            {
                Id = 2,
                Codigo = "INICIAL",
                NombreSuscripcion = "Plan Inicial",
                Caracteristicas = new CaracteristicasSucripcion
                {
                    TimbresIncluidos = 100,
                    EmisoresIncluidos = 1,
                    Vigencia = 1,
                    UnidadVigencia = "Años",
                    SoportePrioritario = false,
                    RespaldoCfdi = false,
                    AccesoApi = false,
                    Recomendado = false
                },
                Precio = 649m,
                CreatedAt = seedDate,
                UpdatedAt = seedDate,
                Guid = "be8919b2-e842-46ed-a172-746967a639f6"
            },
            new Suscripcion
            {
                Id = 3,
                Codigo = "BASICO",
                NombreSuscripcion = "Plan Básico",
                Caracteristicas = new CaracteristicasSucripcion
                {
                    TimbresIncluidos = 200,
                    EmisoresIncluidos = 3,
                    Vigencia = 1,
                    UnidadVigencia = "Años",
                    SoportePrioritario = false,
                    RespaldoCfdi = false,
                    AccesoApi = false,
                    Recomendado = false
                },
                Precio = 1099m,
                CreatedAt = seedDate,
                UpdatedAt = seedDate,
                Guid = "2317a364-c2f9-4651-ad32-1f2fb4a86ade"
            },
            new Suscripcion
            {
                Id = 4,
                Codigo = "PRO",
                NombreSuscripcion = "Plan PRO",
                Caracteristicas = new CaracteristicasSucripcion
                {
                    TimbresIncluidos = 500,
                    EmisoresIncluidos = 10,
                    Vigencia = 2,
                    UnidadVigencia = "Años",
                    SoportePrioritario = true,
                    RespaldoCfdi = true,
                    AccesoApi = true,
                    Recomendado = true
                },
                Precio = 1999m,
                CreatedAt = seedDate,
                UpdatedAt = seedDate,
                Guid = "a5c16c34-605d-4b79-ad61-18c53fff0b5a"
            },
            new Suscripcion
            {
                Id = 5,
                Codigo = "PREMIUM",
                NombreSuscripcion = "Premium",
                Caracteristicas = new CaracteristicasSucripcion
                {
                    TimbresIncluidos = 1000,
                    EmisoresIncluidos = -1,
                    Vigencia = 2,
                    UnidadVigencia = "Años",
                    SoportePrioritario = true,
                    RespaldoCfdi = true,
                    AccesoApi = true,
                    Recomendado = false
                },
                Precio = 3299m,
                CreatedAt = seedDate,
                UpdatedAt = seedDate,
                Guid = "e5e2bcac-0412-4f73-b2ba-bc5ee1e580d8"
            }
        );
    }
}
