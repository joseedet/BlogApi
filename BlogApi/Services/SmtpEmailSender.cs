using System;
using System.Net;
using System.Net.Mail;
using BlogApi.Services.Interfaces;

namespace BlogApi.Services;

/// <summary>
/// Envio de email
/// </summary>
public class SmtpEmailSender : IEmailSender
{
    private readonly IEmailSettingsService _emailSettingsService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="emailSettingsService"></param>
    public SmtpEmailSender(IEmailSettingsService emailSettingsService)
    {
        _emailSettingsService = emailSettingsService;
    }

    /// <summary>
    /// Envio de email
    /// </summary>
    /// <param name="destinatario"></param>
    /// <param name="asunto"></param>
    /// <param name="cuerpo"></param>
    /// <returns></returns>/
    public async Task EnviarAsync(string destinatario, string asunto, string cuerpo)
    {
        var settings = await _emailSettingsService.ObtenerEntidadAsync();
        using var client = new SmtpClient(settings.Host)
        {
            Port = settings.Puerto,
            EnableSsl = settings.UsarSSL,
            Credentials = new NetworkCredential(settings.Usuario, settings.Password),
        };
        var message = new MailMessage
        {
            From = new MailAddress(settings.Remitente, settings.NombreRemitente),
            Subject = asunto,
            Body = cuerpo,
            IsBodyHtml = true,
        };
        message.To.Add(destinatario);
        await client.SendMailAsync(message);
    }
}
