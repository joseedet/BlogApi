using System.Text.Json;
using BlogApi.Models;
using BlogApi.Utils;

namespace BlogApi.Domain.Factories;

public static class NotificacionFactory
{
    /// <summary>
    /// Crea una notificación de nuevo post
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <param name="postId"></param>
    /// <param name="titulo"></param>
    /// <returns>Notificación de nuevo post</returns>
    /// </summary>
    public static Notificacion NuevoPost(int usuarioId, int postId, string titulo) =>
        new Notificacion
        {
            UsuarioId = usuarioId,
            Tipo = TipoNotificacion.NuevoPost,
            Mensaje = $"Has publicado un nuevo post: {titulo}",
            Payload = JsonSerializer.Serialize(new { postId }),
        };

    /// <summary>
    /// Crea una notificación de nuevo comentario
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <param name="postId"></param>
    /// <param name="contenido"></param>
    /// <returns>Notificación de nuevo comentario</returns>
    /// </summary>
    public static Notificacion NuevoComentario(
        int usuarioId,
        int postId,
        string contenido,
        int comentarioId
    ) =>
        new Notificacion
        {
            UsuarioId = usuarioId,
            Tipo = TipoNotificacion.NuevoComentario,
            Mensaje = $"Nuevo comentario en tu post: {contenido}",
            Payload = JsonSerializer.Serialize(new { postId, comentarioId }),
        };

    /// <summary>
    /// Crea una notificación de respuesta a un comentario
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <param name="comentarioId"></param>
    /// <returns>Notificación de respuesta a comentario</returns>
    /// </summary>
    public static Notificacion RespuestaComentario(
        int usuarioId,
        int comentarioId,
        string contenido
    ) =>
        new Notificacion
        {
            UsuarioId = usuarioId,
            Tipo = TipoNotificacion.RespuestaComentario,
            Mensaje = "Alguien respondió a tu comentario",
            Payload = JsonSerializer.Serialize(new { comentarioId, contenido }),
        };

    /// <summary>
    /// Crea una notificación del sistema
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <param name="mensaje"></param>
    /// <returns>Notificación del sistema</returns>
    /// </summary>
    public static Notificacion Sistema(int usuarioId, string mensaje) =>
        new Notificacion
        {
            UsuarioId = usuarioId,
            Tipo = TipoNotificacion.Sistema,
            Mensaje = mensaje,
        };
}
