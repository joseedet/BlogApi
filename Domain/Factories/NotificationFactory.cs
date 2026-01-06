using System.Text.Json;
using BlogApi.Models;
using BlogApi.Utils;

namespace BlogApi.Domain.Factories;

public static class NotificacionFactory
{
    public static Notificacion NuevoPost(int usuarioId, int postId, string titulo) =>
        new Notificacion
        {
            UsuarioId = usuarioId,
            Tipo = TipoNotificacion.NuevoPost,
            Mensaje = $"Has publicado un nuevo post: {titulo}",
            Payload = JsonSerializer.Serialize(new { postId }),
        };

    public static Notificacion NuevoComentario(int usuarioId, int postId, string contenido) =>
        new Notificacion
        {
            UsuarioId = usuarioId,
            Tipo = TipoNotificacion.NuevoComentario,
            Mensaje = $"Nuevo comentario en tu post: {contenido}",
            Payload = JsonSerializer.Serialize(new { postId }),
        };

    public static Notificacion RespuestaComentario(int usuarioId, int comentarioId) =>
        new Notificacion
        {
            UsuarioId = usuarioId,
            Tipo = TipoNotificacion.RespuestaComentario,
            Mensaje = "Alguien respondió a tu comentario",
            Payload = JsonSerializer.Serialize(new { comentarioId }),
        };

    public static Notificacion Sistema(int usuarioId, string mensaje) =>
        new Notificacion
        {
            UsuarioId = usuarioId,
            Tipo = TipoNotificacion.Sistema,
            Mensaje = mensaje,
        };
}
