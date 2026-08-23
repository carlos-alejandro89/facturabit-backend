using System;
using System.ComponentModel.DataAnnotations;
using FacturaBit.Domain.Interfaces;

namespace FacturaBit.Domain.Entities
{
	public class Accounts : IEntity
	{
		[Key] 
		public int Id { get; set; }
		
		public int RolId { get; set; }
		public virtual Rol Rol { get; set; }
		
		public int RedComercialId { get; set; }
		public virtual RedComercial RedComercial { get; set; }
		
        [Required(ErrorMessage = "El nombre es requerido")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "El correo electrònico es obligatorio")]
		public string Email { get; set; }
		public string Telefono { get; set; }
		
		public string Password { get; set; }
		public DateTime PasswordExpiresAt { get; set; }

		public string Guid { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

        
	}
}

