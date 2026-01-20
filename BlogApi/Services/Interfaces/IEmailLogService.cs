using System;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Interfaza para los logos del email
/// </summary>
public interface IEmailLogService
{
    /// <summary>
    /// Registra los envios exitosos
    /// </summary>
    /// <param name="destinatario"></param>
    /// <param name="asunto"></param>
    /// <param name="proveedor"></param>
    /// <returns></returns>
    Task RegistrarExitoAsync(string destinatario, string asunto, string proveedor);

    /// <summary>
    /// Registra los email errados
    /// </summary>
    /// <param name="destinatario"></param>
    /// <param name="asunto"></param>
    /// <param name="proveedor"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    Task RegistrarErrorAsync(string destinatario, string asunto, string proveedor, string error);

    /// <summary>
    /// Obtiene los últimos logs
    /// </summary>
    /// <param name="cantidad"></param>
    /// <returns>Lista de EmailLog</returns>
    Task<List<EmailLog>> ObtenerUltimosAsync(int cantidad);
}
