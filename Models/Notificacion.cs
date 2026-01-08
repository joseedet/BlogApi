using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Utils;

namespace BlogApi.Models;

/// <summary>
/// Modelo de Notificación
/// </summary>
public class Notificacion
{
    /// <summary>
    ///  ID de la notificación
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///    ID del usuario asociado a la notificación
    /// </summary>
    public int UsuarioId { get; set; }

    /// <summary>
    ///     Usuario asociado a la notificación
    /// </summary>
    public Usuario Usuario { get; set; } = null;

    /// <summary>
    ///   Mensaje de la notificación
    /// </summary>
    public required string Mensaje { get; set; }

    /// <summary>
    ///   Fecha de creación de la notificación
    /// </summary>
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    /// <summary>
    ///   Indica si la notificación ha sido leída
    /// </summary>/
    public bool Leida { get; set; } = false;

    /// <summary>
    /// ///  Tipo de notificación
    /// </summary>
    public TipoNotificacion Tipo { get; set; }

    // Opcional: datos adicionales (JSON)
    /// <summary>
    ///   Payload adicional en formato JSON
    /// </summary>
    public string? Payload { get; set; }

    /// <summary>
    ///  ID del usuario destino de la notificación
    /// </summary>
    public int UsuarioDestinoId { get; set; }

    /// <summary>
    /// ID del usuario origen de la notificación
    /// </summary>
    public int UsuarioOrigenId { get; set; }

    /// <summary>
    /// ID del post asociado a la notificación
    /// </summary>
    public int? PostId { get; set; }

    /// <summary>
    /// ID del comentario asociado a la notificación
    /// </summary>
    public int? ComentarioId { get; set; }
}
