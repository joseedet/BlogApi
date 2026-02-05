using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.Services.Interfaces;
/// <summary>
///Interfaz servicio de email
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Método para enviar email
    /// </summary>
    /// <param name="toEmail"></param>
    /// <param name="subject"></param>
    /// <param name="message"></param>
    /// <returns>Task</returns>
    Task EnviarAsync(string toEmail, string subject, string message);

    /// <summary>
    /// Envia email con enlace para la recuperación de la contraseña
    /// </summary>
    /// <param name="email"></param>
    /// <param name="urlRecuperacion"></param>
    /// <returns></returns>
    Task EnviarEmailRecuperacionPasswordAsync(string email, string urlRecuperacion);

    /// <summary>
    /// Email de verificación
    /// </summary>
    /// <param name="emailDestino"></param>
    /// <param name="tokenPlano"></param>
    /// <returns></returns>
    Task EnviarEmailVerificacionAsync(string emailDestino, string tokenPlano);
}
