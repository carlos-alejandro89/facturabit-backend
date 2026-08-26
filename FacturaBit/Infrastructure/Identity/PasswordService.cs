using System.Security.Cryptography;
using FacturaBit.Application.Abstractions;
using FacturaBit.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FacturaBit.Infrastructure.Identity;

public sealed class PasswordService : IPasswordService
{
    private readonly IPasswordHasher<Accounts> _passwordHasher;

    public PasswordService(IPasswordHasher<Accounts> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string CrearHash(Accounts account, string password)
    {
        return _passwordHasher.HashPassword(account, password);
    }

    public bool Verificar(Accounts account, string password)
    {
        PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(
            account,
            account.Password,
            password);

        return result is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
    }

    public string GenerarPasswordTemporal()
    {
        const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lower = "abcdefghijkmnopqrstuvwxyz";
        const string digits = "23456789";
        const string symbols = "!@$%*-_";
        const string all = upper + lower + digits + symbols;

        char[] password = new char[16];
        password[0] = upper[RandomNumberGenerator.GetInt32(upper.Length)];
        password[1] = lower[RandomNumberGenerator.GetInt32(lower.Length)];
        password[2] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
        password[3] = symbols[RandomNumberGenerator.GetInt32(symbols.Length)];

        for (int index = 4; index < password.Length; index++)
        {
            password[index] = all[RandomNumberGenerator.GetInt32(all.Length)];
        }

        RandomNumberGenerator.Shuffle(password.AsSpan());
        return new string(password);
    }
}
