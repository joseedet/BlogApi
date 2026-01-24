using System.Text.Json;
using BlogApi.Models;
using BlogApi.Utils;

namespace BlogApi.Domain.Factories;

public static class NotificacionFactory
{
    // ------------------------------------------------------------
    // Nuevo Post
    // ------------------------------------------------------------
    public static Notificacion NuevoPost(
        int usuarioDestinoId,
        int usuarioOrigenId,
        int postId,
        string titulo
    ) =>
        new Notificacion
        {
            UsuarioDestinoId = usuarioDestinoId,
            UsuarioOrigenId = usuarioOrigenId,
            Tipo = TipoNotificacion.NuevoPost,
            Mensaje = $"Has publicado un nuevo post: {titulo}",
            FechaCreacion = DateTime.UtcNow,
            Leida = false,
            PostId = postId,
            Payload = JsonSerializer.Serialize(new { postId }),
        };

    // ------------------------------------------------------------
    // Nuevo Comentario
    // ------------------------------------------------------------
    public static Notificacion NuevoComentario(
        int usuarioDestinoId,
        int usuarioOrigenId,
        int postId,
        int comentarioId,
        string contenido
    ) =>
        new Notificacion
        {
            UsuarioDestinoId = usuarioDestinoId,
            UsuarioOrigenId = usuarioOrigenId,
            Tipo = TipoNotificacion.NuevoComentario,
            Mensaje = $"Nuevo comentario en tu post: {contenido}",
            FechaCreacion = DateTime.UtcNow,
            Leida = false,
            PostId = postId,
            ComentarioId = comentarioId,
            Payload = JsonSerializer.Serialize(new { postId, comentarioId }),
        };

    public static Notificacion NuevoComentario(
        int usuarioDestinoId,
        int usuarioOrigenId,
        int postId,
        int comentarioId,
        string contenido,
        string autorComentario
    ) =>
        new Notificacion
        {
            UsuarioDestinoId = usuarioDestinoId,
            UsuarioOrigenId = usuarioOrigenId,
            Tipo = TipoNotificacion.NuevoComentario,
            Mensaje = $"Nuevo comentario de {autorComentario}: {contenido}",
            FechaCreacion = DateTime.UtcNow,
            Leida = false,
            PostId = postId,
            Payload = JsonSerializer.Serialize(new { postId, comentarioId }),
        };

    // ------------------------------------------------------------
    // Respuesta a comentario
    // ------------------------------------------------------------
    public static Notificacion RespuestaComentario(
        int usuarioDestinoId,
        int usuarioOrigenId,
        int postId,
        int comentarioId,
        string contenido
    ) =>
        new Notificacion
        {
            UsuarioDestinoId = usuarioDestinoId,
            UsuarioOrigenId = usuarioOrigenId,
            Tipo = TipoNotificacion.RespuestaComentario,
            Mensaje = "Alguien respondió a tu comentario",
            FechaCreacion = DateTime.UtcNow,
            Leida = false,
            PostId = postId,
            ComentarioId = comentarioId,
            Payload = JsonSerializer.Serialize(new { comentarioId, postId }),
        };

    // ------------------------------------------------------------
    // Respuesta a comentario
    // ------------------------------------------------------------
    public static Notificacion RespuestaComentario(
        int usuarioDestinoId,
        int usuarioOrigenId,
        int postId,
        int comentarioId,
        string contenido,
        string autorComentario
    ) =>
        new Notificacion
        {
            UsuarioDestinoId = usuarioDestinoId,
            UsuarioOrigenId = usuarioOrigenId,
            Tipo = TipoNotificacion.RespuestaComentario,
            Mensaje = "Alguien respondió a tu comentario",
            FechaCreacion = DateTime.UtcNow,
            Leida = false,
            PostId = postId,
            ComentarioId = comentarioId,
            //autorComentario = autorComentario,
            Payload = JsonSerializer.Serialize(new { comentarioId, postId }),
        };

    // ------------------------------------------------------------
    // Notificación del sistema
    // ------------------------------------------------------------
    public static Notificacion Sistema(int usuarioDestinoId, string mensaje) =>
        new Notificacion
        {
            UsuarioDestinoId = usuarioDestinoId,
            UsuarioOrigenId = 0, // Sistema
            Tipo = TipoNotificacion.Sistema,
            Mensaje = mensaje,
            FechaCreacion = DateTime.UtcNow,
            Leida = false,
        };
}
