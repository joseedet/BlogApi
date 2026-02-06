using System;

namespace BlogApi.Models;

/// <summary>
/// Log de acceso
/// </summary>
public class AccessLog
{
    /// <summary>
    /// Identificador
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Identificador del usuario
    /// </summary>
    public int? UsuarioId { get; set; }
    /// <summary>
    /// Usuario
    /// </summary>
    public Usuario? Usuario { get; set; }
    /// <summary>
    /// Ruta
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Método
    /// </summary>
    public string Metodo { get; set; } = string.Empty;
    /// <summary>
    /// Si ha sido permitido o denegado
    /// </summary>
    public string Resultado { get; set; } = string.Empty; // Permitido / Denegado 

    /// <summary>
    /// Ip
    /// </summary>
    public string? Ip { get; set; }

    /// <summary>
    /// User-Agent
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Fecha
    /// </summary>
    public DateTime Fecha { get; set; }
}
