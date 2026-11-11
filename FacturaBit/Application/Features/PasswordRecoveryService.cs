using System.Security.Cryptography;
using System.Text;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.Exceptions;
using FacturaBit.Domain.Entities;
using FacturaBit.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace FacturaBit.Application.Features;

public sealed class PasswordRecoveryService : IPasswordRecoveryService
{
    private const string InvalidCodeMessage = "El código es inválido, expiró o superó el número de intentos permitidos.";
    private readonly IPasswordRecoveryRepository _repository;
    private readonly IPasswordService _passwordService;
    private readonly IEmailService _emailService;
    private readonly PasswordRecoveryOptions _options;
    private readonly ILogger<PasswordRecoveryService> _logger;

    public PasswordRecoveryService(
        IPasswordRecoveryRepository repository,
        IPasswordService passwordService,
        IEmailService emailService,
        IOptions<PasswordRecoveryOptions> options,
        ILogger<PasswordRecoveryService> logger)
    {
        _repository = repository;
        _passwordService = passwordService;
        _emailService = emailService;
        _options = options.Value;
        _logger = logger;
    }

    public async Task SolicitarAsync(string email, CancellationToken cancellationToken)
    {
        Accounts? account = await _repository.ObtenerCuentaAsync(email, cancellationToken);
        if (account is null) return;

        DateTime now = DateTime.UtcNow;
        await _repository.InvalidarCodigosActivosAsync(account.Id, now, cancellationToken);

        string code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        string salt = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var recoveryCode = new PasswordRecoveryCode
        {
            AccountId = account.Id,
            CodeHash = CrearHash(account.Email, code, salt),
            Salt = salt,
            ExpiresAt = now.AddMinutes(_options.ExpirationMinutes),
            Guid = Guid.NewGuid().ToString(),
            CreatedAt = now,
            UpdatedAt = now
        };

        await _repository.CrearAsync(recoveryCode, cancellationToken);
        await _repository.GuardarAsync(cancellationToken);
        try
        {
            await _emailService.EnviarCodigoRecuperacionAsync(
                account.Email,
                account.FullName,
                code,
                _options.ExpirationMinutes,
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "No fue posible enviar el código de recuperación a la cuenta {AccountId}.",
                account.Id);
            throw new EmailDeliveryException(
                "No fue posible enviar el correo de recuperación. Inténtalo nuevamente más tarde.",
                exception);
        }
    }

    public async Task<bool> VerificarAsync(
        string email,
        string code,
        CancellationToken cancellationToken)
    {
        Accounts? account = await _repository.ObtenerCuentaAsync(email, cancellationToken);
        if (account is null) return false;

        PasswordRecoveryCode? recoveryCode = await ObtenerCodigoValidoAsync(
            account,
            code,
            cancellationToken);
        return recoveryCode is not null;
    }

    public async Task RestablecerAsync(
        string email,
        string code,
        string newPassword,
        CancellationToken cancellationToken)
    {
        Accounts? account = await _repository.ObtenerCuentaAsync(email, cancellationToken);
        if (account is null) throw new PasswordRecoveryException(InvalidCodeMessage);

        PasswordRecoveryCode? recoveryCode = await ObtenerCodigoValidoAsync(
            account,
            code,
            cancellationToken);
        if (recoveryCode is null) throw new PasswordRecoveryException(InvalidCodeMessage);
        if (!string.IsNullOrWhiteSpace(account.Password) &&
            _passwordService.Verificar(account, newPassword))
            throw new PasswordRecoveryException(
                "La contraseña nueva debe ser diferente de la contraseña actual.");

        DateTime now = DateTime.UtcNow;
        account.Password = _passwordService.CrearHash(account, newPassword);
        account.PasswordExpiresAt = now.AddDays(_options.PasswordValidityDays);
        account.UpdatedAt = now;
        recoveryCode.ConsumedAt = now;
        recoveryCode.UpdatedAt = now;
        await _repository.GuardarAsync(cancellationToken);
    }

    private async Task<PasswordRecoveryCode?> ObtenerCodigoValidoAsync(
        Accounts account,
        string code,
        CancellationToken cancellationToken)
    {
        DateTime now = DateTime.UtcNow;
        PasswordRecoveryCode? recoveryCode = await _repository.ObtenerCodigoActivoAsync(
            account.Id,
            now,
            cancellationToken);
        if (recoveryCode is null || recoveryCode.FailedAttempts >= _options.MaxFailedAttempts)
            return null;

        string candidateHash = CrearHash(account.Email, code, recoveryCode.Salt);
        bool valid = CryptographicOperations.FixedTimeEquals(
            Convert.FromHexString(recoveryCode.CodeHash),
            Convert.FromHexString(candidateHash));
        if (valid) return recoveryCode;

        recoveryCode.FailedAttempts++;
        recoveryCode.UpdatedAt = now;
        if (recoveryCode.FailedAttempts >= _options.MaxFailedAttempts)
            recoveryCode.ConsumedAt = now;
        await _repository.GuardarAsync(cancellationToken);
        return null;
    }

    private string CrearHash(string email, string code, string salt)
    {
        if (string.IsNullOrWhiteSpace(_options.HashKey))
            throw new InvalidOperationException("PasswordRecovery:HashKey no está configurado.");

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_options.HashKey));
        byte[] payload = Encoding.UTF8.GetBytes(
            $"{email.Trim().ToUpperInvariant()}:{code}:{salt}");
        return Convert.ToHexString(hmac.ComputeHash(payload));
    }
}
