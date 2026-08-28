using FacturaBit.Application.Abstractions;
using FacturaBit.Application.DTOs;
using FacturaBit.Application.Exceptions;
using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Features;

public sealed class ApiClientService : IApiClientService
{
    public const string EmitScope = "cfdi.emit";
    private static readonly HashSet<string> AllowedScopes = new(StringComparer.Ordinal) { EmitScope };
    private readonly IApiClientRepository _repository;
    private readonly IApiClientSecretService _secretService;
    private readonly IApplicationTokenGenerator _tokenGenerator;

    public ApiClientService(IApiClientRepository repository, IApiClientSecretService secretService, IApplicationTokenGenerator tokenGenerator)
    {
        _repository = repository;
        _secretService = secretService;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<ApiClientCreatedDto> CrearAsync(string redComercialGuid, string nombre, IReadOnlyCollection<string>? scopes, CancellationToken cancellationToken)
    {
        RedComercial red = await _repository.ObtenerRedComercialAsync(redComercialGuid, cancellationToken)
            ?? throw new EntidadNoEncontradaException("La organización no existe.");
        if (!await _repository.TieneAccesoApiAsync(red.Id, cancellationToken))
            throw new ConflictoException("La suscripción actual no incluye acceso a la API.");

        string[] normalizedScopes = NormalizeScopes(scopes is { Count: > 0 } ? scopes : [EmitScope]);
        string clientId;
        do clientId = _secretService.GenerarClientId();
        while (await _repository.ExisteClientIdAsync(clientId, cancellationToken));

        string secret = _secretService.GenerarClientSecret();
        DateTime now = DateTime.UtcNow;
        var entity = new ApiClient
        {
            RedComercialId = red.Id,
            Nombre = nombre.Trim(),
            ClientId = clientId,
            Scopes = string.Join(' ', normalizedScopes),
            Activo = true,
            Guid = Guid.NewGuid().ToString(),
            CreatedAt = now,
            UpdatedAt = now
        };
        entity.ClientSecretHash = _secretService.CrearHash(entity, secret);
        await _repository.AgregarAsync(entity, cancellationToken);
        return new ApiClientCreatedDto(entity.Guid, entity.Nombre, entity.ClientId, secret, normalizedScopes, entity.CreatedAt);
    }

    public async Task<IReadOnlyCollection<ApiClientDto>> ObtenerAsync(string redComercialGuid, CancellationToken cancellationToken)
    {
        RedComercial red = await _repository.ObtenerRedComercialAsync(redComercialGuid, cancellationToken)
            ?? throw new EntidadNoEncontradaException("La organización no existe.");
        return (await _repository.ObtenerPorRedAsync(red.Id, cancellationToken)).Select(Map).ToArray();
    }

    public async Task<bool> RevocarAsync(string redComercialGuid, string guid, CancellationToken cancellationToken)
    {
        RedComercial? red = await _repository.ObtenerRedComercialAsync(redComercialGuid, cancellationToken);
        if (red is null) return false;
        ApiClient? client = await _repository.ObtenerPorGuidAsync(guid, red.Id, true, cancellationToken);
        if (client is null) return false;
        client.Activo = false;
        client.UpdatedAt = DateTime.UtcNow;
        await _repository.GuardarCambiosAsync(cancellationToken);
        return true;
    }

    public async Task<ClientAccessTokenDto?> AutenticarAsync(string clientId, string clientSecret, string? requestedScope, CancellationToken cancellationToken)
    {
        ApiClient? client = await _repository.ObtenerPorClientIdAsync(clientId.Trim(), true, cancellationToken);
        if (client is null || !client.Activo
            || client.ExpiresAt is DateTime expiresAt && expiresAt <= DateTime.UtcNow
            || !_secretService.Verificar(client, clientSecret)) return null;

        string[] granted = client.Scopes.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        string[] requested = string.IsNullOrWhiteSpace(requestedScope)
            ? granted
            : NormalizeScopes(requestedScope.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        if (requested.Except(granted, StringComparer.Ordinal).Any()) return null;

        client.LastUsedAt = DateTime.UtcNow;
        client.UpdatedAt = client.LastUsedAt.Value;
        await _repository.GuardarCambiosAsync(cancellationToken);
        return _tokenGenerator.Generar(client, string.Join(' ', requested));
    }

    public Task<bool> PuedeEmitirAsync(string redComercialId, string rfcEmisor, CancellationToken cancellationToken)
        => int.TryParse(redComercialId, out int id)
            ? _repository.EmisorPerteneceAsync(id, rfcEmisor, cancellationToken)
            : Task.FromResult(false);

    private static string[] NormalizeScopes(IEnumerable<string> scopes)
    {
        string[] values = scopes.Select(scope => scope.Trim().ToLowerInvariant()).Where(scope => scope.Length > 0).Distinct().ToArray();
        if (values.Length == 0 || values.Any(scope => !AllowedScopes.Contains(scope)))
            throw new ArgumentException("Uno o más scopes solicitados no son válidos.");
        return values;
    }

    private static ApiClientDto Map(ApiClient item) => new(item.Guid, item.Nombre, item.ClientId,
        item.Scopes.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries), item.Activo,
        item.ExpiresAt, item.LastUsedAt, item.CreatedAt);
}
