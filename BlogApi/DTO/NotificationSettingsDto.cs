using System;

namespace BlogApi.DTO;

/// <summary>
/// Ajuste notificaciones Data Transfer Object
/// </summary>
public class NotificationSettingsDto
{
    /// <summary>
    /// Se envía correo por comentario
    /// </summary>
    public bool SendEmailOnComment { get; set; }

    /// <summary>
    /// Se envia correo al administrador de mensajes
    /// </summary>
    public bool SendEmailOnAdminMessage { get; set; }

    /// <summary>
    /// Se envia email al sistema de alertas
    /// </summary>
    public bool SendEmailOnSystemAlert { get; set; }
}
