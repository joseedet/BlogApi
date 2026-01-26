using System.Net;
using System.Net.Mail;
using BlogApi.Services.Interfaces;

namespace BlogApi.Services;

/// <summary>
/// Clase de servicio de email
/// </summary>
public class EmailService : IEmailService
{
    private readonly IEmailSettingsService _settingsService;
    private readonly IEmailTemplateService _templateService;
    private readonly IEmailLogService _logService;

    /// <summary>
    /// Constructor de EmailService
    /// </summary>
    /// <param name="settingsService"></param>
    /// <param name="templateService"></param>
    /// <param name="logService"></param>
    public EmailService(
        IEmailSettingsService settingsService,
        IEmailLogService logService,
        IEmailTemplateService templateService
    )
    {
        _settingsService = settingsService;
        _logService = logService;
        _templateService = templateService;
    }

    /// <summary>
    /// Enviar email.
    /// </summary>
    /// <param name="toEmail"></param>
    /// <param name="subject"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task EnviarAsync(string toEmail, string subject, string message)
    {
        // Obtener configuración desde la BD
        var settings = await _settingsService.ObtenerEntidadAsync();

        try
        {
            using var smtp = new SmtpClient(settings.Host)
            {
                Port = settings.Puerto,
                Credentials = new NetworkCredential(settings.Usuario, settings.Password),
                EnableSsl = settings.UsarSSL,
            };
            using var mail = new MailMessage
            {
                From = new MailAddress(settings.Remitente, settings.NombreRemitente),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };
            mail.To.Add(toEmail);
            await smtp.SendMailAsync(mail);
            await _logService.RegistrarExitoAsync(toEmail, subject, "SMTP");
        }
        catch (Exception ex)
        {
            await _logService.RegistrarErrorAsync(toEmail, subject, "SMTP", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Envia el correo con la plantilla seleccionada
    /// /// </summary>
    /// <param name="destinatario"></param>
    /// <param name="asunto"></param>
    /// <param name="nombrePlantilla"></param>
    /// <param name="variables"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task EnviarConPlantillaAsync(
        string destinatario,
        string asunto,
        string nombrePlantilla,
        Dictionary<string, string> variables
    )
    {
        var settings = await _settingsService.ObtenerEntidadAsync();
        if (!settings.Activo)
            throw new InvalidOperationException("El envío de emails está desactivado.");

        var plantilla = await _templateService.CargarPlantillaAsync(nombrePlantilla);
        var html = _templateService.ReemplazarVariables(plantilla, variables);
        try
        {
            using var smtp = new SmtpClient(settings.Host)
            {
                Port = settings.Puerto,
                Credentials = new NetworkCredential(settings.Usuario, settings.Password),
                EnableSsl = settings.UsarSSL,
            };
            using var mail = new MailMessage
            {
                From = new MailAddress(settings.Remitente, settings.NombreRemitente),
                Subject = asunto,
                Body = html,
                IsBodyHtml = true,
            };
            mail.To.Add(destinatario);
            await smtp.SendMailAsync(mail);
            await _logService.RegistrarExitoAsync(destinatario, asunto, "SMTP");
        }
        catch (Exception ex)
        {
            await _logService.RegistrarErrorAsync(destinatario, asunto, "SMTP", ex.Message);
            throw;
        }
    }
}
