using FacturaBit.Application.DTOs;

namespace FacturaBit.Application.Abstractions;

public interface ICfdiXmlBuilder
{
    string Construir(InvoiceModelDto invoice, DateTime fechaEmision, string sello, bool esGlobal, string certificado, string numeroCertificado);
}
