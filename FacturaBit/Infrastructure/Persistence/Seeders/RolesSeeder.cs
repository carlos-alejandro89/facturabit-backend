using FacturaBit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FacturaBit.Infrastructure.Persistence.Seeders;


    public static class RolesSeeder
    {
        public static void SeedRoles(this ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2026, 8, 21, 20, 21, 33, DateTimeKind.Utc);
            modelBuilder.Entity<Rol>().HasData(
                new Rol{ Id = 1, Nombre = "SUPERADMIN", CreatedAt = seedDate, UpdatedAt = seedDate, Guid = "f0e47c96-4708-46b6-a914-1830e2cfd252" },
                new Rol { Id = 2, Nombre = "ADMIN", CreatedAt = seedDate, UpdatedAt = seedDate, Guid = "582d00c4-cd5b-48c0-8c14-0735db2b8866" },
                new Rol { Id = 3, Nombre = "FACTURISTA", CreatedAt = seedDate, UpdatedAt = seedDate, Guid = "2c04f5e5-9398-4f9a-b880-f0c01476e334" }
            );
        }
    }
