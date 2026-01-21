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
    private readonly IEmailLogService _logService;
    private readonly IEmailTemplateService _templateService;
    /// <summary>
    /// Constructor de EmailService
    /// </summary>
    /// <param name="settingsService"></param>
    /// <param name="sendGridClient"></param>
    /// <param name="logService"></param>
    public EmailService(
        IEmailSettingsService settingsService,
        ISendGridClientFactory sendGridClient,
        IEmailLogService logService,
        IEmailTemplateService templateService;
    )
    {
        _settingsService = settingsService;
        _clientFactory = sendGridClient;
        _logService = logService;
        _templateService=templateService;
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
            var client = _clientFactory.Create(settings.Password);

            var from = new EmailAddress(settings.Remitente, settings.NombreRemitente);
            var to = new EmailAddress(toEmail);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);

            await client.SendEmailAsync(msg);
            await _logService.RegistrarExitoAsync(toEmail, subject, "SendGrid");
        }
        catch (Exception ex)
        {
            await _logService.RegistrarErrorAsync(toEmail, subject, "SendGrid", ex.Message);
            throw;
        }
    }
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

        if (
            string.IsNullOrWhiteSpace(settings.Password)
            || string.IsNullOrWhiteSpace(settings.Remitente)
        )
        {
            throw new InvalidOperationException("La configuración de email no está completa.");
        }

        // 1. Cargar plantilla
        var plantilla = await _templateService.CargarPlantillaAsync(nombrePlantilla);

        // 2. Reemplazar variables
        var html = _templateService.ReemplazarVariables(plantilla, variables);

        // 3. Crear cliente SendGrid
        var client = _clientFactory.Create(settings.Password);

        var from = new EmailAddress(settings.Remitente, settings.NombreRemitente);
        var to = new EmailAddress(destinatario);

        // 4. Crear email HTML
        var msg = MailHelper.CreateSingleEmail(from, to, asunto, null, html);

        // 5. Enviar
        await client.SendEmailAsync(msg);
    }
}
