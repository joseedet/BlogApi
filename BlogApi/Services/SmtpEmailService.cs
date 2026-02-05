using System.Net;
using System.Net.Mail;
using BlogApi.Models;
using BlogApi.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace BlogApi.Services;

/// <summary>
/// Servicio para enviar correos electrónicos usando SMTP
/// </summary>
public class SmtpEmailService : IEmailService
{
    /// <summary>
    /// Configuración de la aplicación
    /// </summary>
    private readonly IConfiguration _config;

    private readonly EmailSettings _settings;

    /// <summary>
    /// Constructor de SmtpEmailService
    /// </summary>
    /// <param name="config"></param>
    public SmtpEmailService(IConfiguration config, IOptions<EmailSettings> settings)
    {
        _config = config;
        _settings = settings.Value; 
    }


    /// <summary>
    /// Envía un correo electrónico
    /// </summary>
    /// <param name="toEmail"></param>
    /// <param name="subject"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <summary>
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
    /// <summary>
    /// Envia Email de recuperación
    /// </summary>
    /// <param name="email"></param>
    /// <param name="urlRecuperacion"></param>
    /// <returns></returns>
    public async Task EnviarEmailRecuperacionPasswordAsync(string email, string urlRecuperacion)
     {
        var asunto = "Recuperación de contraseña";
        var cuerpo = $@" <p>Has solicitado recuperar tu contraseña.</p>
       <p>Haz clic en el siguiente enlace para continuar:</p> <p><a href=""{urlRecuperacion}"">Recuperar contraseña</a></p> <p>Si no has solicitado este cambio, puedes ignorar este mensaje.</p> ";
        await EnviarAsync(email, asunto, cuerpo);
        }


/// <summary>
/// Enviar Email Verificacion Async
/// </summary>
/// <param name="emailDestino"></param>
/// <param name="tokenPlano"></param>
/// <returns></returns>
/// <exception cref="InvalidOperationException"></exception>
    public async Task EnviarEmailVerificacionAsync(string emailDestino, string tokenPlano)
    {
        if (!_settings.Activo) throw new InvalidOperationException("La cuenta de email no está activa.");
        var mensaje = new MailMessage {
            From = new MailAddress(_settings.Remitente, _settings.NombreRemitente),
            Subject = "Verificación de correo",
            Body = GenerarCuerpoEmail(tokenPlano),
            IsBodyHtml = true
        }; mensaje.To.Add(emailDestino);
        using var smtp = new SmtpClient(_settings.Host, _settings.Puerto)
        {
            Credentials = new NetworkCredential(_settings.Usuario, _settings.Password),
            EnableSsl = _settings.UsarSSL
        };
        await smtp.SendMailAsync(mensaje);
    } 
     private string GenerarCuerpoEmail(string token)
    {
        return $@" <h2>Verificación de correo</h2> <p>Gracias por registrarte.
          Para verificar tu cuenta,
           haz clic en el siguiente enlace:</p> <p> <a href=""https://tudominio.com/auth/verify-email?token={token}"" style=""padding:10px 20px; background:#4CAF50; color:white; text-decoration:none; border-radius:5px;""> Verificar mi correo </a> </p> <p>Este enlace expirará en 12 horas.</p> <p>Si no solicitaste esta verificación, puedes ignorar este mensaje.</p> "; 
    }
}
