using System;
using System.ComponentModel.DataAnnotations;

namespace BlogApi.Models;

/// <summary>
/// Ajustes de notificación
/// </summary>
public class NotificationSettings
{
    /// <summary>
    /// Identificador de ajuste de la notificación
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Se envía correo por comentario
    /// </summary>
    [Required]
    public bool SendEmailOnComment { get; set; } = true;

    /// <summary>
    /// Se envia correo al administrador de mensajes
    /// </summary>
    [Required]
    public bool SendEmailOnAdminMessage { get; set; } = true;

    /// <summary>
    /// Se envia email al sistema de alertas
    /// </summary>
    [Required]
    public bool SendEmailOnSystemAlert { get; set; } = true;

    /// <summary>
    /// Si esta activo los ajuste de notificación
    /// </summary>
    [Required]
    public bool Activo { get; set; } = true;
}
