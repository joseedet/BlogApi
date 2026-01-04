using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BlogApi.Services;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _config;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="config"></param>
    public SmtpEmailService(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Envía un correo electrónico utilizando SMTP
    /// </summary>
    /// <param name="toEmail"></param>
    /// <param name="subject"></param>
    /// /// <param name="message"></param>
    /// <returns>Task</returns>
    public async Task EnviarAsync(string toEmail, string subject, string message)
    {
        var host = _config["Smtp:Host"];
        var port = int.Parse(_config["Smtp:Port"]);
        var enableSsl = bool.Parse(_config["Smtp:EnableSsl"]);
        var user = _config["Smtp:User"];
        var password = _config["Smtp:Password"];
        var fromEmail = _config["Smtp:FromEmail"];
        var fromName = _config["Smtp:FromName"];
        var smtp = new SmtpClient(host)
        {
            Port = port,
            Credentials = new NetworkCredential(user, password),
            EnableSsl = enableSsl,
        };
        var mail = new MailMessage
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
