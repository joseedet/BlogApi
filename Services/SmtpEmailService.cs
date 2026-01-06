using System.Net;
using System.Net.Mail;

namespace BlogApi.Services;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _config;

    public SmtpEmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task EnviarAsync(string toEmail, string subject, string message)
    {
        var host =
            _config["Smtp:Host"]
            ?? throw new InvalidOperationException("Smtp:Host no está configurado.");

        var portString =
            _config["Smtp:Port"]
            ?? throw new InvalidOperationException("Smtp:Port no está configurado.");

        if (!int.TryParse(portString, out var port))
            throw new InvalidOperationException("Smtp:Port no es un número válido.");

        var enableSslString =
            _config["Smtp:EnableSsl"]
            ?? throw new InvalidOperationException("Smtp:EnableSsl no está configurado.");

        if (!bool.TryParse(enableSslString, out var enableSsl))
            throw new InvalidOperationException("Smtp:EnableSsl no es un booleano válido.");

        var user =
            _config["Smtp:User"]
            ?? throw new InvalidOperationException("Smtp:User no está configurado.");

        var password =
            _config["Smtp:Password"]
            ?? throw new InvalidOperationException("Smtp:Password no está configurado.");

        var fromEmail =
            _config["Smtp:FromEmail"]
            ?? throw new InvalidOperationException("Smtp:FromEmail no está configurado.");

        var fromName =
            _config["Smtp:FromName"]
            ?? throw new InvalidOperationException("Smtp:FromName no está configurado.");

        using var smtp = new SmtpClient(host)
        {
            Port = port,
            Credentials = new NetworkCredential(user, password),
            EnableSsl = enableSsl,
        };

        using var mail = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = subject,
            Body = message,
            IsBodyHtml = true,
        };

        mail.To.Add(toEmail);

        await smtp.SendMailAsync(mail);
    }
}
