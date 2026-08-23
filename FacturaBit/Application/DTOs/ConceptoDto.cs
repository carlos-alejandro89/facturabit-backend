using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FacturaBit.Application.DTOs
{
    public class ConceptoDto
    {
        [Required(ErrorMessage = "El campo ClaveProdServ es obligatorio.")]
        public string ClaveProdServ { get; set; }

        [Required(ErrorMessage = "El campo NoIdentificacion es obligatorio.")]
        public string NoIdentificacion { get; set; }

        [Required(ErrorMessage = "El campo Descripcion es obligatorio.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El campo Cantidad es obligatorio.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El campo ClaveUnidad es obligatorio.")]
        public string ClaveUnidad { get; set; }

        [Required(ErrorMessage = "El campo Unidad es obligatorio.")]
        public string Unidad { get; set; }

        [Required(ErrorMessage = "El campo ValorUnitario es obligatorio.")]
        public double ValorUnitario { get; set; }

        [Required(ErrorMessage = "El campo Importe es obligatorio.")]
        public double Importe { get; set; }

        [Required(ErrorMessage = "El campo ObjetoImp es obligatorio.")]
        public string ObjetoImp { get; set; }

        [Required(ErrorMessage = "El campo Descuento es obligatorio.")]
        public double Descuento { get; set; }

        [Required(ErrorMessage = "El campo Impuestos es obligatorio.")]
        public List<ImpuestoDto> Impuestos { get; set; }

        [JsonIgnore]
        public List<ImpuestoDto>? Retenciones { get; set; }
    }
}
