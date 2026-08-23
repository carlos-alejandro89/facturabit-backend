namespace FacturaBit.Application.DTOs
{
    public class NominaTrabajadorDto
    {
        public string NumEmpleado { get; set; }
        public string RfcReceptor { get; set; }
        public string Receptor { get; set; }
        public string CveRegimenReceptor { get; set; }
        public string? RegimenReceptor { get; set; }
        public string? DomicilioFiscalReceptor { get; set; }
        public string Puesto { get; set; }
        public string Curp { get; set; }
        public string ClaveEntFed {  get; set; }    
        public string NumSeguridadSocial { get; set; }
        public DateTime FechaInicioRelLaboral { get; set; }
        public string Antiguedad { get; set; }
        public string RegimenContratacion { get; set; }
        public string TipoContrato { get; set; }
        public string RiesgoPuesto { get; set; }
        public string PeriodicidadPago { get; set; }
        public string SalarioDiarioIntegrado { get; set; }
    }
}
