namespace FacturaBit.Application.DTOs
{
    public class InvoiceModelDto
    {
        public string? Serie { get; set; }
        public string FolioInterno { get; set; }
        public DateTime Fecha { get; set; }
        public string CveMetodoPago { get; set; }
        public string MetodoPago { get; set; }
        public string CveFormaPago { get; set; }
        public string FormaPago { get; set; }
        public double SubTotal { get; set; }
        public double Descuentos { get; set; }
        public double Impuestos { get; set; }
        public double Total { get; set; }
        public string RfcEmisor { get; set; }
        public string Emisor { get; set; }
        public string CveRegimenEmisor { get; set; }
        public string RegimenEmisor { get; set; }
        public string LugarExpedicion { get; set; }
        public string RfcReceptor { get; set; }
        public string Receptor { get; set; }
        public string CveRegimenReceptor { get; set; }
        public string RegimenReceptor { get; set; }
        public string DomicilioFiscalReceptor { get; set; }
        public string CveUsoCFDI { get; set; }
        public string UsoCFDI { get; set; }
        public string? Periodicidad { get; set; }
        public string? Meses { get; set; }
        public string? YYYY { get; set; }
        public List<ConceptoDto> Conceptos { get; set; }
    }
}
