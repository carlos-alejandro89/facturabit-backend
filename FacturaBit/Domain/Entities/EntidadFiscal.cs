using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FacturaBit.Domain.Interfaces;

namespace FacturaBit.Domain.Entities;
[Table("entidades_fiscales")]
	public class EntidadFiscal : IEntity
	{
		[Key]
		public int Id { get; set; }
		
		public int? SatRegimenFiscalId { get; set; }
        [Required(ErrorMessage ="El nombre o razón social es obligatorio")]
		public string RazonSocial { get; set; }

        [Required(ErrorMessage = "El RFC es obligatorio")]
        public string Rfc { get; set; }

        public string? RegistroPatronal { get; set; }
        public string? Calle { get; set; }
        public string? NumExt { get; set; }
        public string? NumInt { get; set; }
        public string? Colonia { get; set; }
        public string? Ciudad { get; set; }
        public string? Estado { get; set; }

        [Required(ErrorMessage = "El Código Postal es obligatorio")]
        public string CodigoPostal { get; set; }
        public int TimbresDisponibles { get; set; }
        public bool EsPrepago { get; set; } = true;
        public string? NombreComercial { get; set; }
        public string? TelefonoComercial { get; set; }
        public string? TelefonoComercial2 { get; set; }
        public string? CorreoComercial { get; set; }
        
        public string Guid { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public SatRegimenFiscal? SatRegimenFiscal { get; set; }
        public ICollection<RedComercialEntidadFiscal> RedesComerciales { get; set; } = [];
        public ICollection<CertificadoSelloDigital> Certificados { get; set; } = [];
    }
