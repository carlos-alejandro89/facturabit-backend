using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Abstractions;

public interface IPasswordRecoveryRepository
{
    Task<Accounts?> ObtenerCuentaAsync(string email, CancellationToken cancellationToken);
    Task InvalidarCodigosActivosAsync(int accountId, DateTime now, CancellationToken cancellationToken);
    Task CrearAsync(PasswordRecoveryCode recoveryCode, CancellationToken cancellationToken);
    Task<PasswordRecoveryCode?> ObtenerCodigoActivoAsync(int accountId, DateTime now, CancellationToken cancellationToken);
    Task GuardarAsync(CancellationToken cancellationToken);
}
