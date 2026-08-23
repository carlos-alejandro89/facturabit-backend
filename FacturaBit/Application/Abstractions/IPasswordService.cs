using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Abstractions;

public interface IPasswordService
{
    string GenerarPasswordTemporal();
    string CrearHash(Accounts account, string password);
}
