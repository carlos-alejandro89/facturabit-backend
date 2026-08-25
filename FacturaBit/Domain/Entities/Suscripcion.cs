using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using FacturaBit.Domain.Interfaces;

namespace FacturaBit.Domain.Entities;

[Table("suscripciones")]
public class Suscripcion : IEntity
{
    [Key]
    public int Id { get; set; }
    public string Codigo { get; set; }
    public string NombreSuscripcion { get; set; }
    
    [Column(TypeName = "jsonb")]
    public CaracteristicasSucripcion Caracteristicas { get; set; }
    public decimal Precio { get; set; }
    
    public string Guid { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CaracteristicasSucripcion
{
    public int TimbresIncluidos { get; set; }
    public int EmisoresIncluidos { get; set; }
    public int Vigencia {get;set;}
    public string UnidadVigencia { get; set; }
    public bool SoportePrioritario { get; set; }
    public bool RespaldoCfdi { get; set; }
    public bool AccesoApi { get; set; }
    public bool Recomendado { get; set; }
}