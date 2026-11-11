using FacturaBit.Application.DTOs;

namespace FacturaBit.Application.Abstractions;

public interface IComplementoPagoXmlBuilder
{
    string Construir(
        ComplementoPagoDto complemento,
        DateTime fechaEmision,
        string sello,
        string certificado,
        string numeroCertificado);
}
