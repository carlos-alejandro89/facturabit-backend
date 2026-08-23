using System;
using System.ComponentModel.DataAnnotations;
using FacturaBit.Domain.Interfaces;

namespace FacturaBit.Domain.Entities
{
	public class SatRegimenFiscal : IEntity
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "La clave SAT del regimen Fiscal es un campo obligatorio")]
		public string CveRegimen { get; set; }

		[Required(ErrorMessage = "La descripción del Regímen Fiscal es un campo obligatorio")]
		public string RegimenFiscal { get; set; }
		
		public string Guid { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}

