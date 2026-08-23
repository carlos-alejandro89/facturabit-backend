namespace FacturaBit.Application.DTOs
{
    public class ComplementoDocumentoDto
    {
        public string Folio {  get; set; }
        public string IdDocumento { get; set; }
        public double ImportePagado { get; set; }
        public double ImporteSaldoAnterior { get; set; }
        public double ImporteSaldoInsoluto { get; set; }
        public int NumParcialidad { get; set; }
        public double BaseDR {  get; set; }
        public double ImporteDR { get; set; }
    }
}
