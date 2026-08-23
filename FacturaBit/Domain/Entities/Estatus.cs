using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FacturaBit.Domain.Interfaces;

namespace FacturaBit.Domain.Entities;

[Table("estatus")]
public class Estatus : IEntity
{
    [Key]
    public int Id { get; set; }
    
    public string NombreEstatus{ get; set; }
    public string DescripcionEstatus{ get; set; }
    
    public string Guid { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}