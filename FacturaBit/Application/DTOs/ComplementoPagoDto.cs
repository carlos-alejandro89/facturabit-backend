namespace FacturaBit.Application.DTOs
{
    public class ComplementoPagoDto
    {
        public string Serie { get; set; }
        public string FolioInterno { get; set; }
        public DateTime Fecha { get; set; }
        public string CveFormaPago { get; set; }
        public string FormaPago { get; set; }
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
        public ComplementoInfoDto ComplementoInfo { get; set; }
  
    }
}
