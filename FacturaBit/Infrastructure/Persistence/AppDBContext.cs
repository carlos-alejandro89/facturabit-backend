using System.Reflection;
using System.Text.Json;
using FacturaBit.Domain.Entities;
using FacturaBit.Domain.Interfaces;
using FacturaBit.Infrastructure.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FacturaBit.Infrastructure.Persistence;

public class AppDBContext  : DbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Solo añade esta línea temporalmente para permitir que EF genere la migración
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // 1. Obtenemos el ensamblado (Assembly) donde están tus modelos
        var assembly = Assembly.GetAssembly(typeof(Accounts));

        // 2. Buscamos todas las clases que implementen IEntity (y que no sean abstractas)
        var entityTypes = assembly.GetTypes()
            .Where(t => typeof(IEntity).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        // 3. Registramos cada una en el ModelBuilder
        foreach (var type in entityTypes)
        {
            modelBuilder.Entity(type);
        }

        modelBuilder.Entity<RedComercialEntidadFiscal>(entity =>
        {
            entity.HasIndex(item => new { item.RedComercialId, item.EntidadFiscalId }).IsUnique();
            entity.HasIndex(item => item.Guid).IsUnique();

            entity.HasOne(item => item.RedComercial)
                .WithMany(item => item.EntidadesFiscales)
                .HasForeignKey(item => item.RedComercialId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(item => item.EntidadFiscal)
                .WithMany(item => item.RedesComerciales)
                .HasForeignKey(item => item.EntidadFiscalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EntidadFiscal>().HasIndex(item => item.Guid).IsUnique();
        modelBuilder.Entity<Accounts>().HasIndex(item => item.Guid).IsUnique();
        modelBuilder.Entity<Accounts>().HasIndex(item => item.Email).IsUnique();
        modelBuilder.Entity<ComprobanteFiscal>().HasIndex(item => item.Guid).IsUnique();
        modelBuilder.Entity<SatMotivosCancelacion>(entity =>
        {
            entity.HasIndex(item => item.Guid).IsUnique();
            entity.HasIndex(item => item.CveMotivo).IsUnique();
        });
        modelBuilder.Entity<ApiClient>(entity =>
        {
            entity.HasIndex(item => item.Guid).IsUnique();
            entity.HasIndex(item => item.ClientId).IsUnique();
            entity.HasOne(item => item.RedComercial)
                .WithMany()
                .HasForeignKey(item => item.RedComercialId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrdenCompraSuscripcion>(entity =>
        {
            entity.HasIndex(item => item.Guid).IsUnique();
            entity.HasIndex(item => item.Folio).IsUnique();

            entity.HasOne(item => item.RedComercial)
                .WithMany()
                .HasForeignKey(item => item.RedComercialId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(item => item.Estatus)
                .WithMany()
                .HasForeignKey(item => item.EstatusId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(item => item.Suscripcion)
                .WithMany()
                .HasForeignKey(item => item.SuscripcionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(item => item.Canal)
                .WithMany()
                .HasForeignKey(item => item.CanalId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Suscripcion>()
            .Property(item => item.Caracteristicas)
            .HasConversion(
                value => JsonSerializer.Serialize(value, JsonSerializerOptions.Default),
                value => JsonSerializer.Deserialize<CaracteristicasSucripcion>(value, JsonSerializerOptions.Default)!);

        modelBuilder.Entity<ComprobanteFiscal>()
            .HasOne(item => item.EstatusCfdi)
            .WithMany()
            .HasForeignKey(item => item.EstatusCfdiId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ComprobanteFiscal>()
            .HasOne(item => item.EstatusCancelacion)
            .WithMany()
            .HasForeignKey(item => item.EstatusCancelacionId)
            .OnDelete(DeleteBehavior.Restrict);

        //Seeders
        modelBuilder.SeedRoles();
        modelBuilder.SeedSatRegimenFiscal();
        modelBuilder.SeedSatMotivosCancelacion();
        modelBuilder.SeedEstatus();
        modelBuilder.SeedSuscripciones();
    }
}
