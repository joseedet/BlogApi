using BlogApi.Data;
using BlogApi.DTO;
using BlogApi.Hubs;
using BlogApi.Models;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

/// <summary>
/// Clase servicio de notificaciones
/// </summary>
public class NotificacionesService : INotificacionesService
{
    private readonly BlogDbContext _db;
    private readonly IHubContext<NotificacionesHub> _hub;
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="db"></param>
    /// <param name="hub"></param>
    public NotificacionesService(BlogDbContext db, IHubContext<NotificacionesHub> hub)
    {
        _db = db;
        _hub = hub;
    }

    // ------------------------------------------------------------
    // Crear notificación
    // ------------------------------------------------------------

    /// <summary>
    /// CreateAsync, creamos la notificación
    /// </summary>
    /// <param name="notificacion"></param>
    /// <returns></returns>
    public async Task CrearAsync(Notificacion notificacion)
    {
        _db.Notificaciones.Add(notificacion);
        await _db.SaveChangesAsync();

        await _hub
            .Clients.User(notificacion.UsuarioDestinoId.ToString())
            .SendAsync("NuevaNotificacion", notificacion);
    }

    // ------------------------------------------------------------
    // Obtener todas las notificaciones del usuario
    // ------------------------------------------------------------

    /// <summary>
    /// Obtenmos una lista de usuarios por id
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns>IEnumerable NotificacionDto</returns>
    public async Task<IEnumerable<NotificacionDto>> ObtenerPorUsuarioAsync(int usuarioId)
    {
        return await _db
            .Notificaciones.Where(n => n.UsuarioDestinoId == usuarioId)
            .OrderByDescending(n => n.Fecha)
            .Select(n => new NotificacionDto
            {
                Id = n.Id,
                UsuarioDestinoId = n.UsuarioDestinoId,
                UsuarioOrigenId = n.UsuarioOrigenId,
                Tipo = n.Tipo,
                PostId = n.PostId,
                ComentarioId = n.ComentarioId,
                Mensaje = n.Mensaje,
                Fecha = n.Fecha,
                Leida = n.Leida,
                Payload = n.Payload,
            })
            .ToListAsync();
    }

    // ------------------------------------------------------------
    // Obtener una notificación por ID
    // ------------------------------------------------------------

    /// <summary>
    /// Obtenemos una notificacion.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Notificación</returns>
    public async Task<Notificacion?> ObtenerPorIdAsync(int id)
    {
        return await _db.Notificaciones.FirstOrDefaultAsync(n => n.Id == id);
    }

    // ------------------------------------------------------------
    // Marcar una notificación como leída
    // ------------------------------------------------------------

    /// <summary>
    /// Marcamos como leidas las nofificaciones
    /// </summary>
    /// <param name="id"></param>
    /// <param name="usuarioId"></param>
    /// <returns>verdadero si están marcadas y falso en caso contrario</returns>
    public async Task<bool> MarcarComoLeidaAsync(int id, int usuarioId)
    {
        var notif = await ObtenerPorIdAsync(id);

        if (notif == null || notif.UsuarioDestinoId != usuarioId)
            return false;

        notif.Leida = true;
        await _db.SaveChangesAsync();
        return true;
    }

    // ------------------------------------------------------------
    // Marcar todas como leídas
    // ------------------------------------------------------------

    /// <summary>
    /// Marcamos todas las notificaciones como leidas
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    public async Task MarcarTodasComoLeidasAsync(int usuarioId)
    {
        var notis = await _db
            .Notificaciones.Where(n => n.UsuarioDestinoId == usuarioId && !n.Leida)
            .ToListAsync();

        foreach (var n in notis)
            n.Leida = true;

        await _db.SaveChangesAsync();
    }

    // ------------------------------------------------------------
    // Eliminar notificación
    // ------------------------------------------------------------

    /// <summary>
    /// Elimina notificación
    /// </summary>
    /// <param name="id"></param>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    public async Task<bool> EliminarAsync(int id, int usuarioId)
    {
        var notif = await ObtenerPorIdAsync(id);

        if (notif == null || notif.UsuarioDestinoId != usuarioId)
            return false;

        _db.Notificaciones.Remove(notif);
        await _db.SaveChangesAsync();
        return true;
    }

    // ------------------------------------------------------------
    // Obtener no leídas
    // ------------------------------------------------------------

    /// <summary>
    /// Obtenmos una lista de NotificacionDto
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns>Lista de Notificaciones</returns>
    public async Task<List<NotificacionDto>> ObtenerNoLeidasAsync(int usuarioId)
    {
        return await _db
            .Notificaciones.Where(n => n.UsuarioDestinoId == usuarioId && !n.Leida)
            .OrderByDescending(n => n.Fecha)
            .Select(n => new NotificacionDto
            {
                Id = n.Id,
                UsuarioDestinoId = n.UsuarioDestinoId,
                UsuarioOrigenId = n.UsuarioOrigenId,
                Tipo = n.Tipo,
                PostId = n.PostId,
                ComentarioId = n.ComentarioId,
                Mensaje = n.Mensaje,
                Fecha = n.Fecha,
                Leida = n.Leida,
                Payload = n.Payload,
            })
            .ToListAsync();
    }

    // ------------------------------------------------------------
    // Obtener paginadas
    // ------------------------------------------------------------

    /// <summary>
    /// Paginación de resultado 
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns>Paginacion de resultado de la NotificacionDto</returns>
    public async Task<PaginacionResultado<NotificacionDto>> GetPaginadasAsync(
        int usuarioId,
        int page,
        int pageSize
    )
    {
        var query = _db
            .Notificaciones.Where(n => n.UsuarioDestinoId == usuarioId)
            .OrderByDescending(n => n.Fecha);

        var total = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new NotificacionDto
            {
                Id = n.Id,
                UsuarioDestinoId = n.UsuarioDestinoId,
                UsuarioOrigenId = n.UsuarioOrigenId,
                Tipo = n.Tipo,
                PostId = n.PostId,
                ComentarioId = n.ComentarioId,
                Mensaje = n.Mensaje,
                Fecha = n.Fecha,
                Leida = n.Leida,
                Payload = n.Payload,
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
