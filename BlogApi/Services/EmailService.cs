using BlogApi.Services.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BlogApi.Services;

/// <summary>
/// Clase de servicio de email
/// </summary>
public class EmailService : IEmailService
{
    private readonly IEmailSettingsService _settingsService;
    private readonly ISendGridClientFactory _clientFactory;

    /// <summary>
    /// Constructor de EmailService
    /// </summary>
    /// <param name="settingsService"></param>
    /// <param name="sendGridClient"></param>
    public EmailService(
        IEmailSettingsService settingsService,
        ISendGridClientFactory sendGridClient
    )
    {
        _settingsService = settingsService;
        _clientFactory = sendGridClient;
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

        if (!settings.Activo)
            throw new InvalidOperationException(
                "El envío de emails está desactivado por el administrador."
            );

        if (
            string.IsNullOrWhiteSpace(settings.Usuario)
            || string.IsNullOrWhiteSpace(settings.Password)
            || string.IsNullOrWhiteSpace(settings.Remitente)
        )
        {
            throw new InvalidOperationException("La configuración de email no está completa.");
        }

        // SendGrid usa API Key en lugar de usuario/contraseña
        var client = _clientFactory.Create(settings.Password); // Password = API Key

        var from = new EmailAddress(settings.Remitente, settings.NombreRemitente);
        var to = new EmailAddress(toEmail);

        var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);

        await client.SendEmailAsync(msg);
    }
}
