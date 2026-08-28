using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FacturaBit.Domain.Interfaces;

namespace FacturaBit.Domain.Entities;

[Table("api_clients")]
public sealed class ApiClient : IEntity
{
    [Key]
    public int Id { get; set; }
    public int RedComercialId { get; set; }
    public RedComercial RedComercial { get; set; } = null!;
    [MaxLength(120)]
    public string Nombre { get; set; } = string.Empty;
    [MaxLength(64)]
    public string ClientId { get; set; } = string.Empty;
    [MaxLength(512)]
    public string ClientSecretHash { get; set; } = string.Empty;
    [MaxLength(500)]
    public string Scopes { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public DateTime? ExpiresAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    [MaxLength(36)]
    public string Guid { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
