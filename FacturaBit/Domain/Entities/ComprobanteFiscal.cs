using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using FacturaBit.Domain.Interfaces;

namespace FacturaBit.Domain.Entities;

[Table("comprobantes_fiscales")]
public class ComprobanteFiscal : IEntity
{
    [Key] 
    public int Id { get; set; }
    
    public int EstatusCfdiId { get; set; }
    public virtual Estatus EstatusCfdi { get; set; }
    
    public int? EstatusCancelacionId { get; set; }
    public virtual Estatus? EstatusCancelacion { get; set; }
    
    public int EntidadFiscalId { get; set; }
    public virtual EntidadFiscal EntidadFiscal { get; set; }
    
    public string Folio { get; set; }
    public string? SerieCfdi { get; set; }
    public string ReceptorRfc { get; set; }
    public string ReceptorNombre { get; set; }
    
    [Column(TypeName = "jsonb")]
    public JsonDocument JsonRequest { get; set; } = null!;
    public string TipoRequest { get; set; }
    
    public bool Success { get; set; }
    public string Mensaje { get; set; }
    
    public string NoCertificadoSat { get; set; }
    public string NoCertificadoEmisor { get; set; } 
    public string FolioFiscal { get; set; }
    public string SelloSat { get; set; } 
    public string SelloEmisor { get; set; }
    public DateTime FechaTimbrado { get; set; } 
    public string CadenaOriginalSat { get; set; }
    
    public string? PathXmlCfdi{ get; set; }
    public string? PathXmlCancelacion { get; set; }
    
    public string Guid { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
