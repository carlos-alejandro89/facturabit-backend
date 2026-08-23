using System.ComponentModel.DataAnnotations.Schema;
using FacturaBit.Domain.Interfaces;

namespace FacturaBit.Domain.Entities;

[Table("roles")]
public class Rol : IEntity
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Guid { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}