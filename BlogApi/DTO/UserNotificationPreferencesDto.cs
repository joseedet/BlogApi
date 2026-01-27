using System;

namespace BlogApi.DTO;

/// <summary>
/// Preferencias de notificación del usuario
/// </summary>
public class UserNotificationPreferencesDto
{
    /// <summary>
    /// Recibir notificaciones por correo electrónico
    /// </summary>
    public bool ReceiveEmailNotifications { get; set; }

    /// <summary>
    /// Recibir notificaciones internas
    /// </summary>
    public bool ReceiveInternalNotifications { get; set; }

    /// <summary>
    /// Notificaciones de comentarios.
    /// </summary>
    public bool NotifyOnComment { get; set; }

    /// <summary>
    /// Recibir notificaciones de mensajes del del admin
    /// </summary>
    public bool NotifyOnAdminMessage { get; set; }

    /// <summary>
    /// Recibir notificaciones de alerta del sistema
    /// </summary>
    public bool NotifyOnSystemAlert { get; set; }
}
