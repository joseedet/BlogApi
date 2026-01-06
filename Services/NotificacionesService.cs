using BlogApi.Data;
using BlogApi.DTO;
using BlogApi.Hubs;
using BlogApi.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

public class NotificacionesService : INotificacionesService
{
    private readonly BlogDbContext _db;
    private readonly IHubContext<NotificacionesHub> _hub;

    public NotificacionesService(BlogDbContext db, IHubContext<NotificacionesHub> hub)
    {
        _db = db;
        _hub = hub;
    }

    public async Task CrearAsync(Notificacion notificacion)
    {
        _db.Notificaciones.Add(notificacion);
        await _db.SaveChangesAsync();

        // Enviar por SignalR si quieres
        await _hub
            .Clients.User(notificacion.UsuarioId.ToString())
            .SendAsync("NuevaNotificacion", notificacion);
    }

    public async Task MarcarTodasComoLeidasAsync(int userId)
    {
        var notis = await _db
            .Notificaciones.Where(n => n.UsuarioId == userId && !n.Leida)
            .ToListAsync();
        foreach (var n in notis)
            n.Leida = true;
        await _db.SaveChangesAsync();
    }

    public async Task<List<NotificacionDto>> ObtenerNoLeidasAsync(int id)
    {
        return await _db
            .Notificaciones.Where(n => n.UsuarioId == id && !n.Leida)
            .OrderByDescending(n => n.Fecha)
            .Select(n => new NotificacionDto
            {
                Id = n.Id,
                Mensaje = n.Mensaje,
                Fecha = n.Fecha,
                Leida = n.Leida,
                UsuarioId = n.UsuarioId,
            })
            .ToListAsync();
    }

    public async Task<PaginacionResultado<NotificacionDto>> GetPaginadasAsync(
        int userId,
        int page,
        int pageSize
    )
    {
        var query = _db
            .Notificaciones.Where(n => n.UsuarioId == userId)
            .OrderByDescending(n => n.Fecha);

        var total = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new NotificacionDto
            {
                Id = n.Id,
                Mensaje = n.Mensaje,
                Fecha = n.Fecha,
                Leida = n.Leida,
                UsuarioId = n.UsuarioId,
            })
            .ToListAsync();

        return new PaginacionResultado<NotificacionDto>
        {
            Items = items,
            PaginaActual = page,
            TotalPaginas = (int)Math.Ceiling(total / (double)pageSize),
            TotalRegistros = total,
        };
    }
}
