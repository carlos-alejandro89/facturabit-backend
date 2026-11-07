namespace FacturaBit.Application.Abstractions;

public interface IEmailService
{
    Task EnviarCodigoRecuperacionAsync(
        string email,
        string fullName,
        string code,
        int expirationMinutes,
        CancellationToken cancellationToken);
}
