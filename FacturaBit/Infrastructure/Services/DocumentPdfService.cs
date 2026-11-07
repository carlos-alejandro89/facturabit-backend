using System.Globalization;
using System.Text;
using System.Xml.Linq;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.Exceptions;

namespace FacturaBit.Infrastructure.Services;

public sealed class DocumentPdfService : IDocumentPdfService
{
    public byte[] GenerarCfdi(string xml)
    {
        try
        {
            XDocument document = XDocument.Parse(xml);
            XElement comprobante = document.Root
                ?? throw new FacturacionException("El XML del CFDI no contiene un comprobante.");
            XElement? emisor = Elemento(document, "Emisor");
            XElement? receptor = Elemento(document, "Receptor");
            XElement? timbre = Elemento(document, "TimbreFiscalDigital");

            List<string> lines =
            [
                "FACTURABIT | REPRESENTACION IMPRESA DEL CFDI",
                $"UUID: {Atributo(timbre, "UUID")}",
                $"Fecha: {Atributo(comprobante, "Fecha")}",
                $"Serie / folio: {Atributo(comprobante, "Serie")} {Atributo(comprobante, "Folio")}",
                "",
                $"Emisor: {Atributo(emisor, "Nombre")}",
                $"RFC emisor: {Atributo(emisor, "Rfc")}",
                $"Receptor: {Atributo(receptor, "Nombre")}",
                $"RFC receptor: {Atributo(receptor, "Rfc")}",
                "",
                $"Subtotal: {Atributo(comprobante, "SubTotal")} MXN",
                $"Descuento: {Atributo(comprobante, "Descuento")} MXN",
                $"Total: {Atributo(comprobante, "Total")} MXN",
                $"Metodo de pago: {Atributo(comprobante, "MetodoPago")}",
                $"Forma de pago: {Atributo(comprobante, "FormaPago")}",
                "",
                "CONCEPTOS"
            ];

            foreach (XElement concepto in document.Descendants().Where(item => item.Name.LocalName == "Concepto"))
            {
                lines.Add($"{Atributo(concepto, "Cantidad")} x {Atributo(concepto, "Descripcion")} | {Atributo(concepto, "Importe")} MXN");
            }

            lines.Add("");
            lines.Add("Este documento es una representacion impresa de un CFDI.");
            return CrearPdf(lines);
        }
        catch (FacturacionException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new FacturacionException("No fue posible generar el PDF del CFDI.", exception);
        }
    }

    public byte[] GenerarAcuseCancelacion(string xml)
    {
        try
        {
            XDocument document = XDocument.Parse(xml);
            XElement acuse = document.Root
                ?? throw new FacturacionException("El XML no contiene un acuse de cancelación.");
            XElement? folios = Elemento(document, "Folios");

            return CrearPdf(
            [
                "FACTURABIT | ACUSE DE CANCELACION",
                "",
                $"Fecha del acuse: {Atributo(acuse, "Fecha")}",
                $"RFC emisor: {Atributo(acuse, "RfcEmisor")}",
                $"UUID: {ValorElemento(folios, "UUID")}",
                $"Estatus UUID: {ValorElemento(folios, "EstatusUUID")}",
                "",
                "El presente documento representa el acuse XML emitido por el servicio de cancelacion.",
                "El XML original conserva la firma y los datos criptograficos de la respuesta."
            ]);
        }
        catch (FacturacionException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new FacturacionException("No fue posible generar el PDF del acuse de cancelación.", exception);
        }
    }

    private static XElement? Elemento(XContainer document, string localName) =>
        document.Descendants().FirstOrDefault(item => item.Name.LocalName == localName);

    private static string Atributo(XElement? element, string localName) =>
        element?.Attributes().FirstOrDefault(item => item.Name.LocalName == localName)?.Value ?? "No disponible";

    private static string ValorElemento(XContainer? container, string localName) =>
        container?.Descendants().FirstOrDefault(item => item.Name.LocalName == localName)?.Value ?? "No disponible";

    private static byte[] CrearPdf(IEnumerable<string> sourceLines)
    {
        List<string> lines = sourceLines.SelectMany(line => Ajustar(Ascii(line), 88)).Take(48).ToList();
        var content = new StringBuilder("BT\n/F1 11 Tf\n0.09 0.31 0.29 rg\n");
        int y = 750;
        for (int index = 0; index < lines.Count; index++)
        {
            if (index == 0)
                content.Append("/F1 17 Tf\n0.09 0.31 0.29 rg\n");
            else if (index == 1)
                content.Append("/F1 11 Tf\n0.10 0.15 0.14 rg\n");

            content.AppendFormat(CultureInfo.InvariantCulture, "1 0 0 1 48 {0} Tm ({1}) Tj\n", y, Escape(lines[index]));
            y -= index == 0 ? 30 : 18;
        }
        content.Append("ET");

        string stream = content.ToString();
        string[] objects =
        [
            "<< /Type /Catalog /Pages 2 0 R >>",
            "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
            "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>",
            $"<< /Length {Encoding.ASCII.GetByteCount(stream)} >>\nstream\n{stream}\nendstream"
        ];

        using var output = new MemoryStream();
        Write(output, "%PDF-1.4\n");
        List<long> offsets = [0];
        for (int index = 0; index < objects.Length; index++)
        {
            offsets.Add(output.Position);
            Write(output, $"{index + 1} 0 obj\n{objects[index]}\nendobj\n");
        }
        long xref = output.Position;
        Write(output, $"xref\n0 {objects.Length + 1}\n0000000000 65535 f \n");
        foreach (long offset in offsets.Skip(1)) Write(output, $"{offset:0000000000} 00000 n \n");
        Write(output, $"trailer\n<< /Size {objects.Length + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF");
        return output.ToArray();
    }

    private static IEnumerable<string> Ajustar(string value, int width)
    {
        if (value.Length == 0) return [string.Empty];
        List<string> result = [];
        while (value.Length > width)
        {
            int cut = value.LastIndexOf(' ', width);
            if (cut <= 0) cut = width;
            result.Add(value[..cut]);
            value = value[cut..].TrimStart();
        }
        result.Add(value);
        return result;
    }

    private static string Ascii(string value) => new(value.Normalize(NormalizationForm.FormD)
        .Where(character => CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark && character <= 127)
        .ToArray());

    private static string Escape(string value) => value.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
    private static void Write(Stream stream, string value) => stream.Write(Encoding.ASCII.GetBytes(value));
}
