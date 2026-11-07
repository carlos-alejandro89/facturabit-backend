using System.Net;
using System.Net.Mail;
using FacturaBit.Application.Abstractions;
using Microsoft.Extensions.Options;

namespace FacturaBit.Infrastructure.Services;

public sealed class SmtpEmailService : IEmailService
{
    private readonly EmailOptions _options;

    public SmtpEmailService(IOptions<EmailOptions> options)
    {
        _options = options.Value;
    }

    public async Task EnviarCodigoRecuperacionAsync(
        string email,
        string fullName,
        string code,
        int expirationMinutes,
        CancellationToken cancellationToken)
    {
        ValidarConfiguracion();
        string safeName = WebUtility.HtmlEncode(fullName);
        string safeCode = WebUtility.HtmlEncode(code);
        using var message = new MailMessage
        {
            From = new MailAddress(_options.FromEmail, _options.FromName),
            Subject = "Código para recuperar tu contraseña",
            IsBodyHtml = true,
            Body = $"""
                <div style="font-family:Arial,sans-serif;background:#f6f8f7;padding:32px;color:#17322f">
                  <div style="max-width:520px;margin:auto;background:white;border-radius:18px;padding:32px;border:1px solid #e2e8e5">
                    <p style="color:#0f8f79;font-size:12px;font-weight:700;letter-spacing:1.5px;text-transform:uppercase">Carti · Acceso seguro</p>
                    <h1 style="font-size:24px;margin:18px 0 10px">Recupera tu contraseña</h1>
                    <p style="color:#65736f;line-height:1.6">Hola {safeName}, utiliza este código para continuar:</p>
                    <div style="margin:26px 0;padding:18px;text-align:center;background:#edf9f5;border-radius:14px;font-size:32px;font-weight:800;letter-spacing:10px;color:#174f49">{safeCode}</div>
                    <p style="color:#65736f;line-height:1.6">El código vence en {expirationMinutes} minutos. Si no solicitaste este cambio, ignora este correo.</p>
                  </div>
                </div>
                """
        };
        message.To.Add(new MailAddress(email));

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            Credentials = new NetworkCredential(_options.Username, _options.Password)
        };
        await client.SendMailAsync(message, cancellationToken);
    }

    private void ValidarConfiguracion()
    {
        if (string.IsNullOrWhiteSpace(_options.Host) ||
            string.IsNullOrWhiteSpace(_options.FromEmail))
            throw new InvalidOperationException("La configuración SMTP de recuperación de contraseña está incompleta.");
    }
}
