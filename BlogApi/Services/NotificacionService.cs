using BlogApi.Data;
using BlogApi.Hubs;
using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Utils;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Servicio para gestionar notificaciones
/// </summary>
[Obsolete("NotificacionService está obsoleto. Usa NotificacionesService en su lugar.")]
public class NotificacionService : INotificacionService
{
    /// <summary>
    /// Contexto de la base de datos
    /// </summary>
    private readonly BlogDbContext _context;
    private readonly IHubContext<NotificacionesHub> _hub;
    private readonly INotificacionRepository _notificacionRepository;

    /// <summary>
    /// Constructor de NotificacionService
    /// </summary>
    /// <param name="context"></param>
    /// <param name="notificacionRepository"></param>
    public NotificacionService(
        BlogDbContext context,
        INotificacionRepository notificacionRepository,
        IHubContext<NotificacionesHub> hub
    )
    {
        _context = context;
        _notificacionRepository = notificacionRepository;
        _hub = hub;
    }

    /// <summary>
    /// Crea una nueva notificación para un usuario
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <param name="mensaje"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Crea una notificación de "Me gusta" en un post
    /// </summary>
    /// <param name="usuarioDestinoId"></param>
    /// <param name="usuarioOrigenId"></param>
    /// <param name="postId"></param>
    /// <returns>Task</returns>
    public async Task CrearNotificacionLikePostAsync(
        int usuarioDestinoId,
        int usuarioOrigenId,
        int postId
    )
    {
        var notificacion = new Notificacion
        {
            UsuarioDestinoId = usuarioDestinoId,
            Mensaje = $"Al usuario {usuarioOrigenId} le gustó tu post.",
            UsuarioOrigenId = usuarioOrigenId,
            Tipo = TipoNotificacion.LikePost,
            PostId = postId,
            Fecha = DateTime.UtcNow,
            Leida = false,
            Payload = $"{{ \"postId\": {postId}, \"usuarioOrigenId\": {usuarioOrigenId} }}",
        };

        await _notificacionRepository.CrearAsync(notificacion);
    }

    /// <summary>
    /// Crea una notificación de "Me gusta" en un comentario
    /// </summary>
    /// <param name="usuarioDestinoId"></param>
    /// <param name="usuarioOrigenId"></param>
    /// <param name="comentarioId"></param>
    /// <returns>Task</returns>
    public async Task CrearNotificacionLikeComentarioAsync(
        int usuarioDestinoId,
        int usuarioOrigenId,
        int comentarioId
    )
    {
        var notificacion = new Notificacion
        {
            UsuarioDestinoId = usuarioDestinoId,
            UsuarioOrigenId = usuarioOrigenId,
            Tipo = TipoNotificacion.LikeComentario,
            ComentarioId = comentarioId,
            Mensaje = $"Al usuario {usuarioOrigenId} le gustó tu comentario.",
            Fecha = DateTime.UtcNow,
            Leida = false,
        };

        await _notificacionRepository.CrearAsync(notificacion);

        // Emitir por SignalR

        await _hub
            .Clients.User(usuarioDestinoId.ToString())
            .SendAsync(
                "NotificacionRecibida",
                new
                {
                    Id = notificacion.Id,
                    Tipo = notificacion.Tipo.ToString(),
                    Mensaje = notificacion.Mensaje,
                    Fecha = notificacion.Fecha,
                    ComentarioId = comentarioId,
                    UsuarioOrigenId = usuarioOrigenId,
                }
            );
    }

    /// <summary>
    /// Marca todas las notificaciones de un usuario como leídas
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns>Task</returns>
    public async Task MarcarTodasComoLeidasAsync(int usuarioId)
    {
        var notificaciones = await _notificacionRepository.ObtenerPorUsuarioAsync(usuarioId);
        foreach (var n in notificaciones.Where(n => !n.Leida))
        {
            n.Leida = true;
        }
        await _context.SaveChangesAsync();
    }
}
