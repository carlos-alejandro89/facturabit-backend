using System.Globalization;
using System.Xml.Linq;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;

namespace FacturaBit.Infrastructure.Services;

public sealed class CfdiXmlBuilder : ICfdiXmlBuilder
{
    private static readonly XNamespace Cfdi = "http://www.sat.gob.mx/cfd/4";
    private static readonly XNamespace Xsi = "http://www.w3.org/2001/XMLSchema-instance";

    public string Construir(
        InvoiceModelDto invoice,
        DateTime fechaEmision,
        string sello,
        bool esGlobal,
        string certificado,
        string numeroCertificado)
    {
        XElement comprobante = new(Cfdi + "Comprobante",
            new XAttribute(XNamespace.Xmlns + "cfdi", Cfdi),
            new XAttribute(XNamespace.Xmlns + "xsi", Xsi),
            new XAttribute(Xsi + "schemaLocation",
                "http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd"),
            new XAttribute("Version", "4.0"),
            string.IsNullOrWhiteSpace(invoice.Serie) ? null : new XAttribute("Serie", invoice.Serie.Trim()),
            new XAttribute("Folio", invoice.FolioInterno),
            new XAttribute("Fecha", fechaEmision.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture)),
            new XAttribute("Sello", sello),
            new XAttribute("FormaPago", invoice.CveFormaPago),
            new XAttribute("NoCertificado", numeroCertificado),
            new XAttribute("Certificado", certificado),
            new XAttribute("SubTotal", Dinero(invoice.SubTotal)),
            invoice.Descuentos > 0 ? new XAttribute("Descuento", Dinero(invoice.Descuentos)) : null,
            new XAttribute("Moneda", "MXN"),
            new XAttribute("Total", Dinero(invoice.Total)),
            new XAttribute("TipoDeComprobante", "I"),
            new XAttribute("Exportacion", "01"),
            new XAttribute("MetodoPago", invoice.CveMetodoPago),
            new XAttribute("LugarExpedicion", invoice.LugarExpedicion));

        if (esGlobal)
        {
            comprobante.Add(new XElement(Cfdi + "InformacionGlobal",
                new XAttribute("Periodicidad", invoice.Periodicidad!),
                new XAttribute("Meses", invoice.Meses!),
                new XAttribute("Año", invoice.YYYY!)));
        }

        comprobante.Add(new XElement(Cfdi + "Emisor",
            new XAttribute("Rfc", invoice.RfcEmisor.Trim().ToUpperInvariant()),
            new XAttribute("Nombre", invoice.Emisor.Trim()),
            new XAttribute("RegimenFiscal", invoice.CveRegimenEmisor)));

        comprobante.Add(new XElement(Cfdi + "Receptor",
            new XAttribute("Rfc", invoice.RfcReceptor.Trim().ToUpperInvariant()),
            new XAttribute("Nombre", invoice.Receptor.Trim()),
            new XAttribute("DomicilioFiscalReceptor", invoice.DomicilioFiscalReceptor.Trim()),
            new XAttribute("RegimenFiscalReceptor", invoice.CveRegimenReceptor.Trim()),
            new XAttribute("UsoCFDI", invoice.CveUsoCFDI)));

        XElement conceptos = new(Cfdi + "Conceptos");
        List<ImpuestoAgrupado> traslados = [];

        foreach (ConceptoDto concepto in invoice.Conceptos)
        {
            XElement conceptoElement = new(Cfdi + "Concepto",
                new XAttribute("ClaveProdServ", concepto.ClaveProdServ),
                new XAttribute("NoIdentificacion", concepto.NoIdentificacion),
                new XAttribute("Cantidad", concepto.Cantidad.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("ClaveUnidad", concepto.ClaveUnidad),
                new XAttribute("Unidad", concepto.Unidad),
                new XAttribute("Descripcion", concepto.Descripcion),
                new XAttribute("ValorUnitario", Decimal6(concepto.ValorUnitario)),
                new XAttribute("Importe", Decimal6(concepto.Importe)),
                concepto.Descuento > 0 ? new XAttribute("Descuento", Decimal6(concepto.Descuento)) : null,
                new XAttribute("ObjetoImp", concepto.ObjetoImp));

            XElement impuestosConcepto = new(Cfdi + "Impuestos");

            if (concepto.Impuestos.Count > 0)
            {
                XElement trasladosElement = new(Cfdi + "Traslados");
                foreach (ImpuestoDto impuesto in concepto.Impuestos)
                {
                    string tasa = Tasa(impuesto.TasaOCuota);
                    trasladosElement.Add(new XElement(Cfdi + "Traslado",
                        new XAttribute("Base", Decimal6(impuesto.BaseImpuesto)),
                        new XAttribute("Impuesto", impuesto.Impuesto),
                        new XAttribute("TipoFactor", "Tasa"),
                        new XAttribute("TasaOCuota", tasa),
                        new XAttribute("Importe", Decimal6(impuesto.ImporteImpuesto))));

                    traslados.Add(new ImpuestoAgrupado(
                        impuesto.Impuesto,
                        tasa,
                        impuesto.BaseImpuesto,
                        impuesto.ImporteImpuesto));
                }
                impuestosConcepto.Add(trasladosElement);
            }

            if (concepto.Retenciones is { Count: > 0 })
            {
                XElement retencionesElement = new(Cfdi + "Retenciones");
                foreach (ImpuestoDto retencion in concepto.Retenciones)
                {
                    retencionesElement.Add(new XElement(Cfdi + "Retencion",
                        new XAttribute("Base", Decimal6(retencion.BaseImpuesto)),
                        new XAttribute("Impuesto", retencion.Impuesto),
                        new XAttribute("TipoFactor", "Tasa"),
                        new XAttribute("TasaOCuota", Tasa(retencion.TasaOCuota)),
                        new XAttribute("Importe", Decimal6(retencion.ImporteImpuesto))));
                }
                impuestosConcepto.Add(retencionesElement);
            }

            if (impuestosConcepto.HasElements)
            {
                conceptoElement.Add(impuestosConcepto);
            }

            conceptos.Add(conceptoElement);
        }

        comprobante.Add(conceptos);

        XElement impuestosGlobales = new(Cfdi + "Impuestos",
            new XAttribute("TotalImpuestosTrasladados", Dinero(invoice.Impuestos)));
        XElement trasladosGlobales = new(Cfdi + "Traslados");

        foreach (var group in traslados.GroupBy(item => new { item.Impuesto, item.Tasa }))
        {
            trasladosGlobales.Add(new XElement(Cfdi + "Traslado",
                new XAttribute("Base", Dinero(group.Sum(item => item.Base))),
                new XAttribute("Impuesto", group.Key.Impuesto),
                new XAttribute("TipoFactor", "Tasa"),
                new XAttribute("TasaOCuota", group.Key.Tasa),
                new XAttribute("Importe", Dinero(group.Sum(item => item.Importe)))));
        }

        impuestosGlobales.Add(trasladosGlobales);
        comprobante.Add(impuestosGlobales);

        XDocument document = new(new XDeclaration("1.0", "utf-8", null), comprobante);
        return document.ToString();
    }

    private static string Dinero(double value) => value.ToString("0.00", CultureInfo.InvariantCulture);
    private static string Decimal6(double value) => value.ToString("0.000000", CultureInfo.InvariantCulture);

    private static string Tasa(string value) =>
        double.Parse(value, CultureInfo.InvariantCulture).ToString("0.000000", CultureInfo.InvariantCulture);

    private sealed record ImpuestoAgrupado(string Impuesto, string Tasa, double Base, double Importe);
}
