using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Data;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

/// <summary>
/// Implementación de la interfaz de IEmaailLogService
/// </summary>
public class EmailLogService : IEmailLogService
{
    private readonly BlogDbContext _db;

    /// <summary>
    /// Constructor clase EmailLogService
    /// </summary>
    /// <param name="db"></param>
    public EmailLogService(BlogDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Registra el exito del envio de emails o correos electrónicos
    /// </summary>
    /// <param name="destinatario"></param>
    /// <param name="asunto"></param>
    /// <param name="proveedor"></param>
    /// <returns></returns>
    public async Task RegistrarExitoAsync(string destinatario, string asunto, string proveedor)
    {
        _db.EmailLogs.Add(
            new EmailLog
            {
                Destinatario = destinatario,
                Asunto = asunto,
                Exito = true,
                Proveedor = proveedor,
            }
        );
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Registra los emails erroneos
    /// </summary>
    /// <param name="destinatario"></param>
    /// <param name="asunto"></param>
    /// <param name="proveedor"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public async Task RegistrarErrorAsync(
        string destinatario,
        string asunto,
        string proveedor,
        string error
    )
    {
        _db.EmailLogs.Add(
            new EmailLog
            {
                Destinatario = destinatario,
                Asunto = asunto,
                Exito = false,
                Error = error,
                Proveedor = proveedor,
            }
        );
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Obtiene los últimos logs de los correos.
    /// </summary>
    /// <param name="cantidad"></param>
    /// <returns>Lista de EmailLog</returns>
    public async Task<List<EmailLog>> ObtenerUltimosAsync(int cantidad)
    {
        return await _db
            .EmailLogs.OrderByDescending(x => x.FechaEnvio)
            .Take(cantidad)
            .ToListAsync();
    }
}
