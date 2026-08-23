namespace FacturaBit.Application.DTOs
{
    public class NominaPercepcionDto
    {
        //TipoPercepcion="001" Clave="001" Concepto="SUELDOS, SALARIOS, RAYAS Y JORNALES" ImporteGravado="718.76" ImporteExento="0"
        public string TipoPercepcion { get; set; }
        public string Clave { get; set; }
        public string Concepto { get; set; }
        public double ImporteGravado { get; set; }
        public double ImporteExento { get; set; }
    }
}
