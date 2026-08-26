using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FacturaBit.Domain.Interfaces;

namespace FacturaBit.Domain.Entities;

[Table("ordenes_compra_suscripcion")]
public class OrdenCompraSuscripcion : IEntity
{
    [Key]
    public int Id { get; set; }
    public string Folio { get; set; }
    
    public int RedComercialId { get; set; }
    public virtual RedComercial RedComercial { get; set; }
    
    public int EstatusId { get; set; }
    public virtual Estatus Estatus { get; set; }
    
    public int SuscripcionId { get; set; }
    public virtual Suscripcion Suscripcion { get; set; }
    
    public int? CanalId { get; set; }
    public virtual CanalPago? Canal { get; set; }

    // Desglose de montos
    public decimal Subtotal { get; set; }
    public decimal Iva { get; set; }
    public decimal Total { get; set; }
    
    public string Guid { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
