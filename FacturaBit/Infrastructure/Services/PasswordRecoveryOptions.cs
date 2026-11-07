namespace FacturaBit.Infrastructure.Services;

public sealed class PasswordRecoveryOptions
{
    public const string SectionName = "PasswordRecovery";
    public int ExpirationMinutes { get; init; } = 10;
    public int MaxFailedAttempts { get; init; } = 5;
    public int PasswordValidityDays { get; init; } = 365;
    public string HashKey { get; init; } = string.Empty;
}

public sealed class EmailOptions
{
    public const string SectionName = "Email";
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; } = 587;
    public bool EnableSsl { get; init; } = true;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FromEmail { get; init; } = string.Empty;
    public string FromName { get; init; } = "Carti";
}
