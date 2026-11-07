namespace FacturaBit.Application.Abstractions;

public interface IDocumentPdfService
{
    byte[] GenerarCfdi(string xml);
    byte[] GenerarAcuseCancelacion(string xml);
}
