namespace BlogApi.Utils;

/// <summary>
/// Tipos de notificaciones disponibles
/// </summary>
public enum TipoNotificacion
{
    /// <summary>
    /// Respuesta a un comentario
    /// </summary>
    RespuestaComentario,

    /// <summary>
    /// Tipos de notificaciones disponibles
    /// </summary>/
    NuevoPost,

    /// <summary>
    /// Tipos de notificaciones disponibles
    /// </summary>
    Moderacion,

    /// <summary>
    /// Sistema
    /// </summary>
    Sistema,

    /// <summary>
    /// Mensaje privado
    /// </summary>
    MensajePrivado,

    /// <summary>
    /// Like a un post
    /// </summary>
    LikePost,

    /// <summary>
    /// Like a un comentario
    /// </summary>
    LikeComentario,

    /// <summary>
    /// /// Nuevo comentario en un post
    /// </summary>
    ComentarioEnPost,

    /// <summary>
    /// Respuesta a un comentario
    /// </summary>
    RespuestaAComentario,
}
