using FacturaBit.Application.DTOs;
using FacturaBit.Domain.Entities;

namespace FacturaBit.Application.Abstractions;

public interface IJwtTokenGenerator
{
    GeneratedTokenDto Generar(Accounts account, bool requiereCambioPassword);
}
