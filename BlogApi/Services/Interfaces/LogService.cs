using System;
using BlogApi.Data;
using BlogApi.Models;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Servicio para registrar logs administrativos, como bloqueos de usuarios, eliminación de contenido, etc.
/// </summary>
public class LogService : ILogService
{
    private readonly BlogDbContext _context;
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context"></param>
    public LogService(BlogDbContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Registra un log administrativo
    /// </summary>
    /// <param name="adminId"></param>
    /// <param name="accion"></param>
    /// <param name="usuarioAfectadoId"></param>
    /// <param name="detalles"></param>
    /// <returns></returns>
    public async Task RegistrarAsync(
        int adminId,
        string accion,
        int? usuarioAfectadoId = null,
        string? detalles = null
    )
    {
        var log = new LogAdmin
        {
            UsuarioAdminId = adminId,
            Accion = accion,
            UsuarioAfectadoId = usuarioAfectadoId,
            Detalles = detalles,
            Fecha = DateTime.UtcNow,
        };
        _context.LogAdmins.Add(log);
        await _context.SaveChangesAsync();
    }
}
