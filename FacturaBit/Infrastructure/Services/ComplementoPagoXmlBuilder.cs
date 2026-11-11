using System.Globalization;
using System.Xml.Linq;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Application.Exceptions;

namespace FacturaBit.Infrastructure.Services;

public sealed class ComplementoPagoXmlBuilder : IComplementoPagoXmlBuilder
{
    private static readonly XNamespace Cfdi = "http://www.sat.gob.mx/cfd/4";
    private static readonly XNamespace Xsi = "http://www.w3.org/2001/XMLSchema-instance";
    private static readonly XNamespace Pago20 = "http://www.sat.gob.mx/Pagos20";

    public string Construir(
        ComplementoPagoDto complemento,
        DateTime fechaEmision,
        string sello,
        string certificado,
        string numeroCertificado)
    {
        Validar(complemento);
        IReadOnlyCollection<ComplementoDocumentoDto> documentos = complemento.ComplementoInfo.Documentos!;
        double baseTraslados = documentos.Sum(documento => documento.BaseDR);
        double ivaTrasladado = documentos.Sum(documento => documento.ImporteDR);

        XElement comprobante = new(Cfdi + "Comprobante",
            new XAttribute(XNamespace.Xmlns + "cfdi", Cfdi),
            new XAttribute(XNamespace.Xmlns + "xsi", Xsi),
            new XAttribute(XNamespace.Xmlns + "pago20", Pago20),
            new XAttribute(Xsi + "schemaLocation",
                "http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd " +
                "http://www.sat.gob.mx/Pagos20 http://www.sat.gob.mx/sitio_internet/cfd/Pagos/Pagos20.xsd"),
            new XAttribute("Version", "4.0"),
            string.IsNullOrWhiteSpace(complemento.Serie)
                ? null
                : new XAttribute("Serie", complemento.Serie.Trim()),
            new XAttribute("Folio", complemento.FolioInterno.Trim()),
            new XAttribute("Fecha", Fecha(fechaEmision)),
            new XAttribute("Sello", sello),
            new XAttribute("NoCertificado", numeroCertificado),
            new XAttribute("Certificado", certificado),
            new XAttribute("SubTotal", "0"),
            new XAttribute("Moneda", "XXX"),
            new XAttribute("Total", "0"),
            new XAttribute("TipoDeComprobante", "P"),
            new XAttribute("Exportacion", "01"),
            new XAttribute("LugarExpedicion", complemento.LugarExpedicion.Trim()));

        comprobante.Add(new XElement(Cfdi + "Emisor",
            new XAttribute("Rfc", complemento.RfcEmisor.Trim().ToUpperInvariant()),
            new XAttribute("Nombre", complemento.Emisor.Trim()),
            new XAttribute("RegimenFiscal", complemento.CveRegimenEmisor.Trim())));

        comprobante.Add(new XElement(Cfdi + "Receptor",
            new XAttribute("Rfc", complemento.RfcReceptor.Trim().ToUpperInvariant()),
            new XAttribute("Nombre", complemento.Receptor.Trim()),
            new XAttribute("DomicilioFiscalReceptor", complemento.DomicilioFiscalReceptor.Trim()),
            new XAttribute("RegimenFiscalReceptor", complemento.CveRegimenReceptor.Trim()),
            new XAttribute("UsoCFDI", "CP01")));

        comprobante.Add(new XElement(Cfdi + "Conceptos",
            new XElement(Cfdi + "Concepto",
                new XAttribute("ClaveProdServ", "84111506"),
                new XAttribute("Cantidad", "1"),
                new XAttribute("ClaveUnidad", "ACT"),
                new XAttribute("Descripcion", "Pago"),
                new XAttribute("ValorUnitario", "0"),
                new XAttribute("Importe", "0"),
                new XAttribute("ObjetoImp", "01"))));

        XElement pago = new(Pago20 + "Pago",
            new XAttribute("FechaPago", Fecha(complemento.ComplementoInfo.FechaPago)),
            new XAttribute("FormaDePagoP", complemento.CveFormaPago.Trim()),
            new XAttribute("MonedaP", "MXN"),
            new XAttribute("TipoCambioP", "1"),
            new XAttribute("Monto", Dinero(complemento.ComplementoInfo.TotalPago)));

        foreach (ComplementoDocumentoDto documento in documentos)
        {
            pago.Add(new XElement(Pago20 + "DoctoRelacionado",
                string.IsNullOrWhiteSpace(documento.Folio)
                    ? null
                    : new XAttribute("Folio", documento.Folio.Trim()),
                new XAttribute("IdDocumento", documento.IdDocumento.Trim().ToUpperInvariant()),
                new XAttribute("MonedaDR", "MXN"),
                new XAttribute("EquivalenciaDR", "1"),
                new XAttribute("NumParcialidad", documento.NumParcialidad),
                new XAttribute("ImpSaldoAnt", Dinero(documento.ImporteSaldoAnterior)),
                new XAttribute("ImpPagado", Dinero(documento.ImportePagado)),
                new XAttribute("ImpSaldoInsoluto", Dinero(documento.ImporteSaldoInsoluto)),
                new XAttribute("ObjetoImpDR", "02"),
                new XElement(Pago20 + "ImpuestosDR",
                    new XElement(Pago20 + "TrasladosDR",
                        new XElement(Pago20 + "TrasladoDR",
                            new XAttribute("BaseDR", Decimal6(documento.BaseDR)),
                            new XAttribute("ImpuestoDR", "002"),
                            new XAttribute("TipoFactorDR", "Tasa"),
                            new XAttribute("TasaOCuotaDR", "0.160000"),
                            new XAttribute("ImporteDR", Decimal6(documento.ImporteDR)))))));
        }

        pago.Add(new XElement(Pago20 + "ImpuestosP",
            new XElement(Pago20 + "TrasladosP",
                new XElement(Pago20 + "TrasladoP",
                    new XAttribute("BaseP", Dinero(baseTraslados)),
                    new XAttribute("ImpuestoP", "002"),
                    new XAttribute("TipoFactorP", "Tasa"),
                    new XAttribute("TasaOCuotaP", "0.160000"),
                    new XAttribute("ImporteP", Dinero(ivaTrasladado))))));

        XElement pagos = new(Pago20 + "Pagos",
            new XAttribute("Version", "2.0"),
            new XElement(Pago20 + "Totales",
                new XAttribute("TotalTrasladosBaseIVA16", Dinero(baseTraslados)),
                new XAttribute("TotalTrasladosImpuestoIVA16", Dinero(ivaTrasladado)),
                new XAttribute("MontoTotalPagos", Dinero(complemento.ComplementoInfo.TotalPago))),
            pago);

        comprobante.Add(new XElement(Cfdi + "Complemento", pagos));
        return new XDocument(new XDeclaration("1.0", "utf-8", null), comprobante).ToString();
    }

    private static void Validar(ComplementoPagoDto complemento)
    {
        List<string> faltantes = [];
        if (string.IsNullOrWhiteSpace(complemento.FolioInterno)) faltantes.Add(nameof(complemento.FolioInterno));
        if (string.IsNullOrWhiteSpace(complemento.CveFormaPago)) faltantes.Add(nameof(complemento.CveFormaPago));
        if (string.IsNullOrWhiteSpace(complemento.RfcEmisor)) faltantes.Add(nameof(complemento.RfcEmisor));
        if (string.IsNullOrWhiteSpace(complemento.Emisor)) faltantes.Add(nameof(complemento.Emisor));
        if (string.IsNullOrWhiteSpace(complemento.CveRegimenEmisor)) faltantes.Add(nameof(complemento.CveRegimenEmisor));
        if (string.IsNullOrWhiteSpace(complemento.LugarExpedicion)) faltantes.Add(nameof(complemento.LugarExpedicion));
        if (string.IsNullOrWhiteSpace(complemento.RfcReceptor)) faltantes.Add(nameof(complemento.RfcReceptor));
        if (string.IsNullOrWhiteSpace(complemento.Receptor)) faltantes.Add(nameof(complemento.Receptor));
        if (string.IsNullOrWhiteSpace(complemento.CveRegimenReceptor)) faltantes.Add(nameof(complemento.CveRegimenReceptor));
        if (string.IsNullOrWhiteSpace(complemento.DomicilioFiscalReceptor)) faltantes.Add(nameof(complemento.DomicilioFiscalReceptor));

        if (faltantes.Count > 0)
        {
            throw new FacturacionException(
                $"Para emitir el complemento de pago son obligatorios los campos: {string.Join(", ", faltantes)}.");
        }

        if (complemento.ComplementoInfo is null)
            throw new FacturacionException("La información del pago es obligatoria.");
        if (complemento.ComplementoInfo.FechaPago == default)
            throw new FacturacionException("La fecha del pago es obligatoria.");
        if (complemento.ComplementoInfo.TotalPago <= 0)
            throw new FacturacionException("El total del pago debe ser mayor que cero.");
        if (complemento.ComplementoInfo.Documentos is not { Count: > 0 })
            throw new FacturacionException("Debe incluir al menos un documento relacionado.");

        foreach (ComplementoDocumentoDto documento in complemento.ComplementoInfo.Documentos)
        {
            if (string.IsNullOrWhiteSpace(documento.IdDocumento))
                throw new FacturacionException("Todos los documentos relacionados deben incluir su UUID.");
            if (documento.NumParcialidad <= 0)
                throw new FacturacionException("El número de parcialidad debe ser mayor que cero.");
            if (documento.ImportePagado <= 0 || documento.ImporteSaldoAnterior < documento.ImportePagado)
                throw new FacturacionException("Los importes del documento relacionado no son válidos.");
            if (Math.Abs((documento.ImporteSaldoAnterior - documento.ImportePagado) - documento.ImporteSaldoInsoluto) > 0.01)
                throw new FacturacionException("El saldo insoluto no coincide con el saldo anterior menos el importe pagado.");
        }

        double totalDocumentos = complemento.ComplementoInfo.Documentos.Sum(item => item.ImportePagado);
        if (Math.Abs(totalDocumentos - complemento.ComplementoInfo.TotalPago) > 0.01)
            throw new FacturacionException("El total del pago no coincide con la suma de los documentos relacionados.");
    }

    private static string Fecha(DateTime value) =>
        value.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);

    private static string Dinero(double value) =>
        value.ToString("0.00", CultureInfo.InvariantCulture);

    private static string Decimal6(double value) =>
        value.ToString("0.000000", CultureInfo.InvariantCulture);
}
