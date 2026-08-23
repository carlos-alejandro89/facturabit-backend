namespace FacturaBit.Application.DTOs
{
    public class NominaOtroPagoDto
    {
        public string TipoOtroPago { get; set; }
        public string Clave { get; set; }
        public string Concepto { get; set; }
        public double Importe { get; set; }

        public double? SubsidioCausado { get; set; }
    }
}
