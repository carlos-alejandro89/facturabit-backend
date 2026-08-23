namespace FacturaBit.Application.DTOs
{
    public class NominaDto
    {
        public string Serie { get; set; }
        public string FolioInterno { get; set; }
        public DateTime Fecha {  get; set; } //Fecha de emisión del documento
        public DateTime FechaInicialPago { get; set; }
        public DateTime FechaFinalPago { get; set; }
        public DateTime FechaPago { get; set; }
        public int DiasPagados { get; set; }       
        public string MetodoPago { get; set; }
        public NominaPatronDto Patron { get; set; }
        public NominaTrabajadorDto Trabajador { get; set; }    
       
       public List<NominaPercepcionDto>   Percepciones { get; set; }
        public List<NominaOtroPagoDto> OtrosPagos { get; set; }
       public List<NominaDeduccionDto> Deducciones { get; set; }    
    }
}
