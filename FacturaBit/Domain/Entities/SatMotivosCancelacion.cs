using System.ComponentModel.DataAnnotations;
using FacturaBit.Domain.Interfaces;

namespace FacturaBit.Domain.Entities;

public class SatMotivosCancelacion : IEntity
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "La clave SAT del motivo de cancelación es obligatoria.")]
    [MaxLength(2)]
    public string CveMotivo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción del motivo de cancelación es obligatoria.")]
    [MaxLength(180)]
    public string MotivoCancelacion { get; set; } = string.Empty;

    public bool RequiereFolioSustitucion { get; set; }

    public string Guid { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
