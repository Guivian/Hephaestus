
using System.Net;
using System.Net.Mail;
using Hephaestus.Api.Options;
using Microsoft.Extensions.Options;

namespace Hephaestus.Api.Services;

public sealed class EmailService(
    IOptions<EmailOptions> options,
    IHostEnvironment environment,
    ILogger<EmailService> logger)
{
    private readonly EmailOptions settings = options.Value;

    public async Task SendTwoFactorCodeAsync(
        string recipientEmail,
        string recipientName,
        string code,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(settings.Host) ||
            string.IsNullOrWhiteSpace(settings.FromAddress))
        {
            if (!environment.IsDevelopment())
                throw new InvalidOperationException("O serviço SMTP não está configurado.");

            logger.LogWarning(
                "APENAS DESENVOLVIMENTO — Código 2FA para {Email}: {Code}",
                recipientEmail,
                code);
            return;
        }

        using var message = new MailMessage
        {
            From = new MailAddress(settings.FromAddress, settings.FromName),
            Subject = "Código de autenticação Hephaestus",
            Body = $"Olá {recipientName},\n\nO seu código de autenticação é: {code}\n\nO código é válido durante 5 minutos.",
            IsBodyHtml = false
        };
        message.To.Add(recipientEmail);

        using var client = new SmtpClient(settings.Host, settings.Port)
        {
            EnableSsl = settings.EnableSsl
        };

        if (!string.IsNullOrWhiteSpace(settings.Username))
            client.Credentials = new NetworkCredential(settings.Username, settings.Password);

        cancellationToken.ThrowIfCancellationRequested();
        await client.SendMailAsync(message, cancellationToken);
    }
}
