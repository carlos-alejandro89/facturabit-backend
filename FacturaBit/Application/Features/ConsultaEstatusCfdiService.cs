using System.Text.Json;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Application.Exceptions;
using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Features;

public sealed class ConsultaEstatusCfdiService : IConsultaEstatusCfdiService
{
    private readonly IComprobanteFiscalRepository _repository;
    private readonly ISatConsultaEstatusClient _satClient;

    public ConsultaEstatusCfdiService(
        IComprobanteFiscalRepository repository,
        ISatConsultaEstatusClient satClient)
    {
        _repository = repository;
        _satClient = satClient;
    }

    public async Task<ConsultaEstatusCfdiResultDto> ConsultarAsync(
        string rfc,
        string uuid,
        string redComercialGuid,
        CancellationToken cancellationToken)
    {
        string rfcNormalizado = rfc?.Trim().ToUpperInvariant() ?? string.Empty;
        if (rfcNormalizado.Length is < 12 or > 13)
            throw new FacturacionException("El RFC del emisor no tiene un formato válido.");

        if (!Guid.TryParse(uuid?.Trim(), out Guid uuidNormalizado))
            throw new FacturacionException("El UUID proporcionado no tiene un formato válido.");

        ComprobanteFiscal? comprobante = await _repository.ObtenerPorUuidRfcYRedComercialAsync(
            uuidNormalizado.ToString("D").ToUpperInvariant(),
            rfcNormalizado,
            redComercialGuid,
            cancellationToken);
        if (comprobante is null)
            throw new FacturacionException(
                "No se encontró el comprobante dentro de la organización autenticada.");

        if (string.IsNullOrWhiteSpace(comprobante.FolioFiscal))
            throw new FacturacionException("El comprobante todavía no cuenta con un UUID timbrado.");

        if (string.IsNullOrWhiteSpace(comprobante.SelloEmisor) || comprobante.SelloEmisor.Length < 8)
            throw new FacturacionException("El comprobante no contiene un sello digital válido para consultar al SAT.");

        decimal total = ObtenerTotal(comprobante.JsonRequest);
        SatConsultaEstatusResultDto satResult = await _satClient.ConsultarAsync(
            new SatConsultaEstatusRequestDto
            {
                RfcEmisor = comprobante.EntidadFiscal.Rfc.Trim().ToUpperInvariant(),
                RfcReceptor = comprobante.ReceptorRfc.Trim().ToUpperInvariant(),
                Total = total,
                Uuid = comprobante.FolioFiscal.Trim().ToUpperInvariant(),
                UltimosOchoSello = comprobante.SelloEmisor[^8..]
            },
            cancellationToken);

        return new ConsultaEstatusCfdiResultDto
        {
            Uuid = comprobante.FolioFiscal.Trim().ToUpperInvariant(),
            Estado = satResult.Estado,
            CodigoEstatus = satResult.CodigoEstatus,
            EsCancelable = satResult.EsCancelable,
            EstatusCancelacion = satResult.EstatusCancelacion,
            ValidacionEfos = satResult.ValidacionEfos,
            ConsultadoEn = DateTime.UtcNow
        };
    }

    private static decimal ObtenerTotal(JsonDocument request)
    {
        if (!request.RootElement.TryGetProperty("invoice", out JsonElement invoice) ||
            (!invoice.TryGetProperty("Total", out JsonElement total) &&
             !invoice.TryGetProperty("total", out total)) ||
            !total.TryGetDecimal(out decimal value) ||
            value <= 0)
        {
            throw new FacturacionException("No fue posible obtener un total válido del CFDI almacenado.");
        }

        return value;
    }
}
