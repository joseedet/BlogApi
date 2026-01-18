using BlogApi.Data;
using BlogApi.DTO;
using BlogApi.Hubs;
using BlogApi.Models;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

/// <summary>
/// Clase NotificacionesService
/// </summary>
public class NotificacionesService : INotificacionesService
{
    private readonly BlogDbContext _db;
    private readonly IHubContext<NotificacionesHub> _hub;
    private readonly IEmailService _email;

    /// <summary>
    /// Constructor de NotificacionesService
    /// </summary>
    /// <param name="db"></param>
    /// <param name="hub"></param>
    /// <param name="email"></param>
    public NotificacionesService(
        BlogDbContext db,
        IHubContext<NotificacionesHub> hub,
        IEmailService email
    )
    {
        _db = db;
        _hub = hub;
        _email = email;
    }

    // ------------------------------------------------------------
    // Crear notificación
    // ------------------------------------------------------------

    /// <summary>
    /// Crea Notificación
    /// </summary>
    /// <param name="notificacion"></param>
    /// <returns></returns>
    public async Task CrearAsync(Notificacion notificacion)
    {
        _db.Notificaciones.Add(notificacion);
        await _db.SaveChangesAsync();

        // SignalR
        await _hub
            .Clients.User(notificacion.UsuarioDestinoId.ToString())
            .SendAsync("NuevaNotificacion", notificacion);

        // Email
        await EnviarEmailNotificacion(notificacion);
    }

    // ------------------------------------------------------------
    // Enviar email (método privado reutilizable)
    // ------------------------------------------------------------

    private async Task EnviarEmailNotificacion(Notificacion notificacion)
    {
        // Obtener email del usuario destino
        var emailDestino = await _db
            .Usuarios.Where(u => u.Id == notificacion.UsuarioDestinoId)
            .Select(u => u.Email)
            .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(emailDestino))
            return;

        var asunto = $"Nueva notificación: {notificacion.Tipo}";
        var cuerpo =
            $@"
            <h2>Tienes una nueva notificación</h2>
            <p>{notificacion.Mensaje}</p>
            <p><small>Fecha: {notificacion.Fecha}</small></p>
        ";

        await _email.EnviarAsync(emailDestino, asunto, cuerpo);
    }

    // ------------------------------------------------------------
    // Marcar todas como leídas
    // ------------------------------------------------------------

    /// <summary>
    /// Marca todas las notificaciones como leídas
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task MarcarTodasComoLeidasAsync(int userId)
    {
        var notis = await _db
            .Notificaciones.Where(n => n.UsuarioDestinoId == userId && !n.Leida)
            .ToListAsync();

        foreach (var n in notis)
            n.Leida = true;

        await _db.SaveChangesAsync();
    }

    // ------------------------------------------------------------
    // Obtener no leídas
    // ------------------------------------------------------------

    /// <summary>
    /// Obtiene las notificaciones no leídas
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns>Lista de notificaciones no leídas</returns>
    public async Task<List<NotificacionDto>> ObtenerNoLeidasAsync(int usuarioId)
    {
        return await _db
            .Notificaciones.Where(n => n.UsuarioDestinoId == usuarioId && !n.Leida)
            .OrderByDescending(n => n.Fecha)
            .Select(n => ToDto(n))
            .ToListAsync();
    }

    // ------------------------------------------------------------
    // Obtener paginadas
    // ------------------------------------------------------------
    public async Task<PaginacionResultado<NotificacionDto>> GetPaginadasAsync(
        int userId,
        int page,
        int pageSize
    )
    {
        var query = _db
            .Notificaciones.Where(n => n.UsuarioDestinoId == userId)
            .OrderByDescending(n => n.Fecha);

        var total = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => ToDto(n))
            .ToListAsync();

        return new PaginacionResultado<NotificacionDto>
        {
            Items = items,
            PaginaActual = page,
            TotalPaginas = (int)Math.Ceiling(total / (double)pageSize),
            TotalRegistros = total,
        };
    }

    // ------------------------------------------------------------
    // Conversión a DTO
    // ------------------------------------------------------------
    private static NotificacionDto ToDto(Notificacion n) =>
        new()
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
        };
}
