using System.ComponentModel.DataAnnotations.Schema;
using FacturaBit.Domain.Interfaces;

namespace FacturaBit.Domain.Entities;

[Table("redes_comerciales_entidades_fiscales")]
public sealed class RedComercialEntidadFiscal : IEntity
{
    public int Id { get; set; }
    public int RedComercialId { get; set; }
    public RedComercial RedComercial { get; set; } = null!;
    public int EntidadFiscalId { get; set; }
    public EntidadFiscal EntidadFiscal { get; set; } = null!;
    public string Guid { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
