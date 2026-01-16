using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Models;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Interfaz para el servicio de notificaciones
/// </summary>
[Obsolete("INotificacionService está obsoleto. Usa INotificacionesService en su lugar.")]
public interface INotificacionService
{
    /// <summary>
    /// Crea una nueva notificación
    /// </summary>
    /// <param name="notificacion"></param>
    /// <returns>Task</returns>
    Task CrearAsync(Notificacion notificacion);

    /// <summary>
    ///
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns>IEnumerable&lt;Notificacion&gt;</returns>
    Task<IEnumerable<Notificacion>> GetByUsuarioAsync(int usuarioId);

    /// <summary>
    /// Marca una notificación como leída
    /// </summary>
    /// <param name="id"></param>
    /// <param name="usuarioId"></param>
    /// <returns>true si se marca como leída, false en caso contrario</returns>
    Task<bool> MarcarComoLeidaAsync(int id, int usuarioId);

    /// <summary>
    /// Crea una notificación de "Me gusta" en un post
    /// </summary>
    /// <param name="usuarioDestinoId"></param>
    /// <param name="usuarioOrigenId"></param>
    /// <param name="postId"></param>
    /// <returns>Task</returns>
    Task CrearNotificacionLikePostAsync(int usuarioDestinoId, int usuarioOrigenId, int postId);

    /// <summary>
    /// Crea una notificación de "Me gusta" en un comentario
    /// </summary>
    /// <param name="usuarioDestinoId"></param>
    /// <param name="usuarioOrigenId"></param>
    /// <param name="comentarioId"></param>
    /// <returns>Task</returns>
    Task CrearNotificacionLikeComentarioAsync(
        int usuarioDestinoId,
        int usuarioOrigenId,
        int comentarioId
    );

    /// <summary>
    /// Marca todas las notificaciones de un usuario como leídas
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns>Task</returns>
    Task MarcarTodasComoLeidasAsync(int usuarioId);
}
