using BlogApi.Services;
using BlogApi.Services.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BlogApi.Services;

public class EmailService : IEmailService
{
    /// <summary>
    /// Constructor de EmailService
    /// </summary>
    private readonly string _apiKey;

    /////// <summary>
    /// Dirección de correo del remitente
    /// </summary>
    /// <param name="fromEmail"></param>
    /// <param name="fromName"></param>
    /// </summary>
    private readonly string _fromEmail;

    /// <summary>
    ///     Nombre del remitente
    /// </summary>
    /// <param name="fromName"></param>
    /// <returns></returns>
    /// </summary>
    private readonly string _fromName;

    /// <summary>
    /// Constructor de EmailService
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    /// <summary>
    public EmailService(IConfiguration config)
    {
        _apiKey = config["SendGrid:ApiKey"];
        _fromEmail = config["SendGrid:FromEmail"];
        _fromName = config["SendGrid:FromName"];
    }

    /// <summary>
    /// Envía un correo electrónico
    /// </summary>
    /// <param name="toEmail"></param>
    /// <param name="subject"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    /// </summary>
    public async Task EnviarAsync(string toEmail, string subject, string message)
    {
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress(_fromEmail, _fromName);
        var to = new EmailAddress(toEmail);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
        await client.SendEmailAsync(msg);
    }
}
