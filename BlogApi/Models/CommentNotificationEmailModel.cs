using System;

namespace BlogApi.Models;

/// <summary>
/// Modelo de notifidción de comentario por emal
/// </summary>
public class CommentNotificationEmailModel
{
    /// <summary>
    /// Nombre de usuario
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Titulo de powt
    /// </summary>
    public string PostTitle { get; set; }

    /// <summary>
    /// /// Dirección de correo electrónico
    /// </summary>
    public string Email { get; set; }
}
