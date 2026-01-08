using BlogApi.Data;
using BlogApi.DTO;
using BlogApi.Hubs;
using BlogApi.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using BlogApi.Services.Interfaces;

namespace BlogApi.Services;

public class NotificacionesService : INotificacionesService
{
    /// <summary>
    /// Constructor de NotificacionesService
    /// </summary>
    private readonly BlogDbContext _db;

    /// <summary>
    /// Hub de notificaciones para SignalR
    /// </summary>
    private readonly IHubContext<NotificacionesHub> _hub;

    /// <summary>
    /// Constructor de NotificacionesService
    /// </summary>
    /// <param name="db"></param>
    /// <param name="hub"></param>
    public NotificacionesService(BlogDbContext db, IHubContext<NotificacionesHub> hub)
    {
        _db = db;
        _hub = hub;
    }

    /// <summary>
    /// Crea una nueva notificación
    /// </summary>
    /// <param name="notificacion"></param>
    /// <returns></returns>
    public async Task CrearAsync(Notificacion notificacion)
    {
        _db.Notificaciones.Add(notificacion);
        await _db.SaveChangesAsync();

        // Enviar por SignalR si quieres
        await _hub
            .Clients.User(notificacion.UsuarioId.ToString())
            .SendAsync("NuevaNotificacion", notificacion);
    }

    /// <summary>
    /// Marca una notificación como leída
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task MarcarTodasComoLeidasAsync(int userId)
    {
        var notis = await _db
            .Notificaciones.Where(n => n.UsuarioId == userId && !n.Leida)
            .ToListAsync();
        foreach (var n in notis)
            n.Leida = true;
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Obtiene las notificaciones no leídas de un usuario
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Lista de notificaciones no leídas</returns>
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

    /// <summary>
    /// Obtiene notificaciones paginadas de un usuario
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    // <returns>Paginación de notificaciones</returns>
    /// </summary>
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
