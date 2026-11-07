using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FacturaBit.Domain.Interfaces;

namespace FacturaBit.Domain.Entities;

[Table("password_recovery_codes")]
public sealed class PasswordRecoveryCode : IEntity
{
    [Key]
    public int Id { get; set; }
    public int AccountId { get; set; }
    public Accounts Account { get; set; } = null!;
    [MaxLength(128)]
    public string CodeHash { get; set; } = string.Empty;
    [MaxLength(64)]
    public string Salt { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? ConsumedAt { get; set; }
    public int FailedAttempts { get; set; }
    [MaxLength(36)]
    public string Guid { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
