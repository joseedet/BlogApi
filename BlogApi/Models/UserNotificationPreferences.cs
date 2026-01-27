using System;

namespace BlogApi.Models;

/// <summary>
/// Prefencias sobre las notificaciones configuración del usuario
/// </summary>
public class UserNotificationPreferences
{
    /// <summary>
    /// Identificador del registro
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Id del usuario
    /// </summary>
    public int UsuarioId { get; set; }

    /// <summary>
    /// Propiedad de navegación del usuario
    /// </summary>
    public Usuario Usuario { get; set; }

    /// <summary>
    /// Recibir notificaciones por email.
    /// </summary>
    // Preferencias generales
    public bool ReceiveEmailNotifications { get; set; } = true;

    /// <summary>
    /// Recibir notificaciones internas
    /// </summary>
    public bool ReceiveInternalNotifications { get; set; } = true;

    /// <summary>
    /// Notificación comentario
    /// </summary>
    // Preferencias por tipo
    public bool NotifyOnComment { get; set; } = true;

    /// <summary>
    /// Notificación del mensaje admin
    /// </summary>
    public bool NotifyOnAdminMessage { get; set; } = true;

    /// <summary>
    /// Notificaciones de alerta del sistema
    /// </summary>
    public bool NotifyOnSystemAlert { get; set; } = true;
}
