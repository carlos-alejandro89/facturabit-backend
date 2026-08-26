using FacturaBit.Application.Abstractions;
using FacturaBit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FacturaBit.Infrastructure.Persistence.Repositories;

public sealed class AuthenticationRepository : IAuthenticationRepository
{
    private readonly AppDBContext _dbContext;

    public AuthenticationRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Accounts?> ObtenerPorEmailAsync(string email, CancellationToken cancellationToken)
    {
        string normalizedEmail = email.Trim().ToUpperInvariant();

        return _dbContext.Set<Accounts>()
            .AsNoTracking()
            .Include(account => account.Rol)
            .Include(account => account.RedComercial)
            .SingleOrDefaultAsync(
                account => account.Email.ToUpper() == normalizedEmail,
                cancellationToken);
    }
}
