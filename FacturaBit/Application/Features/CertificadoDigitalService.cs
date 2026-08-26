using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Features;

public sealed class CertificadoDigitalService : ICertificadoDigitalService
{
    private readonly ICertificadoSelloDigitalRepository _repository;

    public CertificadoDigitalService(ICertificadoSelloDigitalRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<CertificadoDigitalDto>?> ObtenerPorEmisorAsync(
        string rfc,
        string redComercialGuid,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<CertificadoSelloDigital>? certificados =
            await _repository.ObtenerPorRfcYRedComercialAsync(rfc, redComercialGuid, cancellationToken);
        if (certificados is null) return null;

        DateTime now = DateTime.UtcNow;
        return certificados.Select(certificado =>
        {
            bool vigente = certificado.ValidoDesde <= now && certificado.ValidoHasta >= now;
            string estado = vigente
                ? "Vigente"
                : certificado.ValidoDesde > now ? "Próximo" : "Vencido";

            return new CertificadoDigitalDto
            {
                Id = certificado.Id,
                ArchivoCer = certificado.ArchivoCer,
                ArchivoKey = certificado.ArchivoKey,
                ValidoDesde = certificado.ValidoDesde,
                ValidoHasta = certificado.ValidoHasta,
                TamanoClave = certificado.TamanoClave,
                NumeroSerie = certificado.NumeroSerie,
                HuellaDigital = certificado.HuellaDigital,
                Estado = estado,
                Vigente = vigente
            };
        }).ToArray();
    }
}
