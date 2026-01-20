using System;

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
    public string Host { get; set; } = string.Empty;
    /// <summary>
    /// Puerto del host
    /// </summary>
    public int Puerto { get; set; }
    /// <summary>
    /// Usuario de email
    /// </summary>
    public string Usuario { get; set; } = string.Empty;

    /// <summary>
    /// Password del usuario de email
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Remitente
    /// </summary>
    public string Remitente { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del remitente
    /// </summary>
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
