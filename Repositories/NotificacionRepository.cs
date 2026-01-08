using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Data;
using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repositories;

public class NotificacionRepository : INotificacionRepository
{
    private readonly BlogDbContext _context;

    public NotificacionRepository(BlogDbContext context)
    {
        _context = context;
    }

    public async Task CrearAsync(Notificacion notificacion)
    {
        _context.Notificaciones.Add(notificacion);
        await _context.SaveChangesAsync();
    }

    public async Task<Notificacion?> ObtenerPorIdAsync(int id)
    {
        return await _context.Notificaciones.FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<IEnumerable<Notificacion>> ObtenerPorUsuarioAsync(int usuarioId)
    {
        return await _context
            .Notificaciones.Where(n => n.UsuarioDestinoId == usuarioId)
            .OrderByDescending(n => n.Fecha)
            .ToListAsync();
    }

    public async Task MarcarComoLeidaAsync(int id)
    {
        var notificacion = await _context.Notificaciones.FindAsync(id);
        if (notificacion != null)
        {
            notificacion.Leida = true;
            await _context.SaveChangesAsync();
        }
    }
}
