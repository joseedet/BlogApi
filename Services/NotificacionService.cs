using BlogApi.Data;
using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

public class NotificacionService : INotificacionService
{
    /// <summary>
    /// Contexto de la base de datos
    /// </summary>
    private readonly BlogDbContext _context;

    /// <summary>
    /// Constructor de NotificacionService
    /// </summary>
    /// <param name="context"></param>
    /// </summary>
    public NotificacionService(BlogDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Crea una nueva notificación para un usuario
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <param name="mensaje"></param>
    /// <returns></returns>
    /// </summary>
    public async Task CrearAsync(int usuarioId, string mensaje)
    {
        var n = new Notificacion { UsuarioId = usuarioId, Mensaje = mensaje };
        _context.Notificaciones.Add(n);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Obtiene las notificaciones de un usuario
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns>IEnumerable de notificaciones</returns>
    /// </summary>
    public async Task<IEnumerable<Notificacion>> GetByUsuarioAsync(int usuarioId)
    {
        return await _context
            .Notificaciones.Where(n => n.UsuarioId == usuarioId)
            .OrderByDescending(n => n.Fecha)
            .ToListAsync();
    }

    /// <summary>
    /// Marca una notificación como leída
    /// </summary>
    /// <param name="id"></param>
    /// <param name="usuarioId"></param>
    /// <returns>bool</returns>
    /// </summary>
    public async Task<bool> MarcarComoLeidaAsync(int id, int usuarioId)
    {
        var n = await _context.Notificaciones.FirstOrDefaultAsync(n =>
            n.Id == id && n.UsuarioId == usuarioId
        );
        if (n == null)
            return false;
        n.Leida = true;
        await _context.SaveChangesAsync();
        return true;
    }
}
