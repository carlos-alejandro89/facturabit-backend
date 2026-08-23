namespace FacturaBit.Application.DTOs
{
    public class CancelacionDto
    {
        public string RFC { get; set; }
        public string UUID { get; set; }
        public string Motivo { get; set; }
        public string CerB64 { get; set; }
        public string KeyB64 { get; set; }
        public string Password { get; set; }
    }
}
