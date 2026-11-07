namespace FacturaBit.Application.Abstractions;

public interface IPasswordRecoveryService
{
    Task SolicitarAsync(string email, CancellationToken cancellationToken);
    Task<bool> VerificarAsync(string email, string code, CancellationToken cancellationToken);
    Task RestablecerAsync(string email, string code, string newPassword, CancellationToken cancellationToken);
}
