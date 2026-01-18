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
}
