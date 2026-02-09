using System.Text.Json;
using BlogApi.Models;
using BlogApi.Utils;

namespace BlogApi.Domain.Factories;

/// <summary>
/// Fábrica para crear objetos de tipo Notificación con diferentes tipos y mensajes predefinidos.
/// </summary>
public static class NotificacionFactory
{
    private static string CrearPayload(Dictionary<string, object> data)
    {
        return JsonSerializer.Serialize(data);
    }

    /// <summary>
    /// Crea una notificación para un nuevo post publicado.
    /// </summary>
    /// <param name="usuarioDestinoId"></param>
    /// <param name="usuarioOrigenId"></param>
    /// <param name="postId"></param>
    /// <param name="titulo"></param>
    /// <returns>Una nueva notificación de tipo NuevoPost</returns>
    // ------------------------------------------------------------
    // NUEVO POST
    // ------------------------------------------------------------
    public static Notificacion NuevoPost(
        int usuarioDestinoId,
        int usuarioOrigenId,
        int postId,
        string titulo
    )
    {
        return new Notificacion
        {
            UsuarioDestinoId = usuarioDestinoId,
            UsuarioOrigenId = usuarioOrigenId,
            Tipo = TipoNotificacion.NuevoPost,
            Mensaje = $"Se ha publicado un nuevo post: {titulo}",
            FechaCreacion = DateTime.UtcNow,
            Payload = CrearPayload(
                new Dictionary<string, object> { { "postId", postId }, { "titulo", titulo } }
            ),
        };
    }

    /// <summary>
    /// Crea una notificación para un nuevo comentario en un post.
    /// </summary>
    /// <param name="usuarioDestinoId"></param>
    /// <param name="usuarioOrigenId"></param>
    /// <param name="postId"></param>
    /// <param name="comentarioId"></param>
    /// <param name="contenido"></param>
    /// <returns>Una nueva notificación de tipo NuevoComentario</returns>
    // ------------------------------------------------------------
    // NUEVO COMENTARIO
    // ------------------------------------------------------------
    public static Notificacion NuevoComentario(
        int usuarioDestinoId,
        int usuarioOrigenId,
        int postId,
        int comentarioId,
        string contenido
    )
    {
        return new Notificacion
        {
            UsuarioDestinoId = usuarioDestinoId,
            UsuarioOrigenId = usuarioOrigenId,
            Tipo = TipoNotificacion.NuevoComentario,
            Mensaje = "Tu post ha recibido un nuevo comentario.",
            FechaCreacion = DateTime.UtcNow,
            Payload = CrearPayload(
                new Dictionary<string, object>
                {
                    { "postId", postId },
                    { "comentarioId", comentarioId },
                    { "contenido", contenido },
                }
            ),
        };
    }

    /// <summary>
    /// Crea una notificación para una respuesta a un comentario.
    /// </summary>
    /// <param name="usuarioDestinoId"></param>
    /// <param name="usuarioOrigenId"></param>
    /// <param name="postId"></param>
    /// <param name="comentarioId"></param>
    /// <param name="contenido"></param>
    /// <param name="autorComentario"></param>
    /// <returns>Una nueva notificación de tipo RespuestaComentario</returns>
    // ------------------------------------------------------------
    // RESPUESTA A COMENTARIO
    // ------------------------------------------------------------
    public static Notificacion RespuestaComentario(
        int usuarioDestinoId,
        int usuarioOrigenId,
        int postId,
        int comentarioId,
        string contenido,
        string autorComentario
    )
    {
        return new Notificacion
        {
            UsuarioDestinoId = usuarioDestinoId,
            UsuarioOrigenId = usuarioOrigenId,
            Tipo = TipoNotificacion.RespuestaComentario,
            Mensaje = $"{autorComentario} ha respondido a tu comentario.",
            FechaCreacion = DateTime.UtcNow,
            Payload = CrearPayload(
                new Dictionary<string, object>
                {
                    { "postId", postId },
                    { "comentarioId", comentarioId },
                    { "contenido", contenido },
                }
            ),
        };
    }

    /// <summary>
    /// Crea una notificación para un comentario aprobado.
    /// </summary>
    /// <param name="usuarioDestinoId"></param>
    /// <param name="comentarioId"></param>
    /// <returns>Una nueva notificación de tipo ComentarioAprobado</returns>
    // ------------------------------------------------------------
    // COMENTARIO APROBADO
    // ------------------------------------------------------------
    public static Notificacion ComentarioAprobado(int usuarioDestinoId, int comentarioId)
    {
        return new Notificacion
        {
            UsuarioDestinoId = usuarioDestinoId,
            UsuarioOrigenId = null,
            Tipo = TipoNotificacion.ComentarioAprobado,
            Mensaje = "Tu comentario ha sido aprobado.",
            FechaCreacion = DateTime.UtcNow,
            Payload = CrearPayload(
                new Dictionary<string, object> { { "comentarioId", comentarioId } }
            ),
        };
    }

    /// <summary>
    /// Crea una notificación para un comentario rechazado.
    /// </summary>
    /// <param name="usuarioDestinoId"></param>
    /// <param name="comentarioId"></param>
    /// <returns>Una nueva notificación de tipo ComentarioRechazado</returns>
    // ------------------------------------------------------------
    // COMENTARIO RECHAZADO
    // ------------------------------------------------------------
    public static Notificacion ComentarioRechazado(int usuarioDestinoId, int comentarioId)
    {
        return new Notificacion
        {
            UsuarioDestinoId = usuarioDestinoId,
            UsuarioOrigenId = null,
            Tipo = TipoNotificacion.ComentarioRechazado,
            Mensaje = "Tu comentario ha sido rechazado.",
            FechaCreacion = DateTime.UtcNow,
            Payload = CrearPayload(
                new Dictionary<string, object> { { "comentarioId", comentarioId } }
            ),
        };
    }

    /// <summary>
    /// Crea una notificación para un mensaje del sistema.
    /// </summary>
    /// <param name="usuarioDestinoId"></param>
    /// <param name="mensaje"></param>
    /// <returns>Una nueva notificación de tipo Sistema</returns>
    // ------------------------------------------------------------
    // MENSAJE DEL SISTEMA
    // ------------------------------------------------------------
    public static Notificacion MensajeSistema(int usuarioDestinoId, string mensaje)
    {
        return new Notificacion
        {
            UsuarioDestinoId = usuarioDestinoId,
            UsuarioOrigenId = null,
            Tipo = TipoNotificacion.Sistema,
            Mensaje = mensaje,
            FechaCreacion = DateTime.UtcNow,
            Payload = "{}",
        };
    }
}
