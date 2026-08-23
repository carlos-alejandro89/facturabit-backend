using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FacturaBit.Domain.Interfaces;

namespace FacturaBit.Domain.Entities;

[Table("redes_comeciales")]
public class RedComercial : IEntity
{
    [Key]
    public int Id { get; set; }
    public string NombreRedComercial { get; set; }
    
    public string Guid { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<RedComercialEntidadFiscal> EntidadesFiscales { get; set; } = [];
}
