using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Utils;

namespace BlogApi.DTO;

/// <summary>
/// Data Transfer Object para Notificación
/// </summary>
public class NotificacionDto
{
    /// <summary>
    /// Identificador de la notificación
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del usuario destinatario de la notificación
    /// </summary>
    public int UsuarioDestinoId { get; set; }

    /// <summary>
    /// Identificador del usuario origen de la notificación
    /// </summary>
    public int UsuarioOrigenId { get; set; }

    /// <summary>
    /// Tipo de notificación
    /// </summary>
    public TipoNotificacion Tipo { get; set; }

    /// <summary>
    /// Identificador del post relacionado (si aplica)
    /// </summary>
    public int? PostId { get; set; }

    /// <summary>
    ///     Identificador del comentario relacionado (si aplica)
    /// </summary>
    public int? ComentarioId { get; set; }

    /// <summary>
    /// Mensaje de la notificación
    /// </summary>
    public string Mensaje { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de creación de la notificación
    /// </summary>
    public DateTime Fecha { get; set; }

    /// <summary>
    /// Indica si la notificación ha sido leída
    /// </summary>
    public bool Leida { get; set; }

    /// <summary>
    /// Payload adicional de la notificación
    /// </summary>
    public string? Payload { get; set; }
}
