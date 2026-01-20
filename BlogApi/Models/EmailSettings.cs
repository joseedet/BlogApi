using System;
using System.ComponentModel.DataAnnotations;

namespace BlogApi.Models;

/// <summary>
/// Clase de configuración del email
/// </summary>
public class EmailSettings
{
    /// <summary>
    /// Id del Email
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Host
    /// </summary>
    [Required, MaxLength(200)]
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// Puerto del host
    /// </summary>
    public int Puerto { get; set; }

    /// <summary>
    /// Usuario de email
    /// </summary>
    [Required, MaxLength(200)]
    public string Usuario { get; set; } = string.Empty;

    /// <summary>
    /// Password del usuario de email
    /// </summary>
    [Required, MaxLength(500)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Remitente
    /// </summary>
    [Required, MaxLength(200)]
    public string Remitente { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del remitente
    /// </summary>
    [MaxLength(200)]
    public string NombreRemitente { get; set; } = string.Empty;

    /// <summary>
    /// Usar email seguro
    /// </summary>
    public bool UsarSSL { get; set; } = true;

    /// <summary>
    /// Está activa la cuenta de email.
    /// </summary>
    public bool Activo { get; set; } = true;
}
