using System;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Innterfaz para envio de email
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Método para enviar email
    /// </summary>
    /// <param name="destinatario"></param>
    /// <param name="asunto"></param>
    /// <param name="cuerpo"></param>
    /// <returns></returns>
    Task EnviarAsync(string destinatario, string asunto, string cuerpo);
}
