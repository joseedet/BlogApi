using BlogApi.Data;
using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

public class NotificacionService : INotificacionService
{
    private readonly BlogDbContext _context;

    public NotificacionService(BlogDbContext context)
    {
        _context = context;
    }

    public async Task CrearAsync(int usuarioId, string mensaje)
    {
        var n = new Notificacion { UsuarioId = usuarioId, Mensaje = mensaje };
        _context.Notificaciones.Add(n);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Notificacion>> GetByUsuarioAsync(int usuarioId)
    {
        return await _context
            .Notificaciones.Where(n => n.UsuarioId == usuarioId)
            .OrderByDescending(n => n.Fecha)
            .ToListAsync();
    }

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
