namespace FacturaBit.Application.DTOs
{
    public class ImpuestosDto
    {
        public String Impuesto;
        public String TasaOCuota;
        public float ImporteImpuesto;
        public float Base;

        public void setImpuesto(String value)
        {
            this.Impuesto = value;
        }

        public String getImpuesto()
        {
            return this.Impuesto;
        }

        public void setTasaOCuota(String value)
        {
            this.TasaOCuota = value;
        }

        public String getTasaOCuota()
        {
            return this.TasaOCuota;
        }

        public void setImporteImpuesto(float valor)
        {
            this.ImporteImpuesto = valor;
        }

        public float getImporteImpuesto()
        {
            return this.ImporteImpuesto;
        }
    }
}
