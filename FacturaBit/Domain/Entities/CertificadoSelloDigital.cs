using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FacturaBit.Domain.Interfaces;

namespace FacturaBit.Domain.Entities;

[Table("certificados_fiscales")]
public class CertificadoSelloDigital : IEntity
{
    [Key]
    public int Id { get; set; }
    
    public int EntidadFiscalId { get; set; }    
    public virtual EntidadFiscal EntidadFiscal { get; set; }
    
    public string ArchivoCer {get; set;} 
    public string ArchivoKey {get; set;} 
    public string PasswordCert {get; set;}
    public DateTime ValidoDesde {get; set;} 
    public DateTime ValidoHasta {get; set;} 
    public int TamanoClave {get; set;} 
    public string NumeroSerie {get; set;} 
    public string HuellaDigital {get; set;} 
}