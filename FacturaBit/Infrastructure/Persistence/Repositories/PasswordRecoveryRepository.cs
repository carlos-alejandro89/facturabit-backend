using FacturaBit.Application.Abstractions;
using FacturaBit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FacturaBit.Infrastructure.Persistence.Repositories;

public sealed class PasswordRecoveryRepository : IPasswordRecoveryRepository
{
    private readonly AppDBContext _dbContext;

    public PasswordRecoveryRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Accounts?> ObtenerCuentaAsync(string email, CancellationToken cancellationToken)
    {
        string normalizedEmail = email.Trim().ToUpperInvariant();
        return _dbContext.Set<Accounts>().SingleOrDefaultAsync(
            account => account.Email.ToUpper() == normalizedEmail,
            cancellationToken);
    }

    public Task InvalidarCodigosActivosAsync(
        int accountId,
        DateTime now,
        CancellationToken cancellationToken) =>
        _dbContext.Set<PasswordRecoveryCode>()
            .Where(item => item.AccountId == accountId && item.ConsumedAt == null)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(item => item.ConsumedAt, now)
                    .SetProperty(item => item.UpdatedAt, now),
                cancellationToken);

    public async Task CrearAsync(
        PasswordRecoveryCode recoveryCode,
        CancellationToken cancellationToken) =>
        await _dbContext.Set<PasswordRecoveryCode>().AddAsync(recoveryCode, cancellationToken);

    public Task<PasswordRecoveryCode?> ObtenerCodigoActivoAsync(
        int accountId,
        DateTime now,
        CancellationToken cancellationToken) =>
        _dbContext.Set<PasswordRecoveryCode>()
            .Where(item => item.AccountId == accountId &&
                           item.ConsumedAt == null &&
                           item.ExpiresAt > now)
            .OrderByDescending(item => item.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public Task GuardarAsync(CancellationToken cancellationToken) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
