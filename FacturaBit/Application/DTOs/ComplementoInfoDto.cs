namespace FacturaBit.Application.DTOs
{
    public class ComplementoInfoDto
    {
        public DateTime FechaPago { get; set; }
        public double BaseTraslados {  get; set; }

        public double IvaTraslado { get; set; }
        public double TotalPago { get; set; }

        public List<ComplementoDocumentoDto>? Documentos { get; set; }
    }
}
