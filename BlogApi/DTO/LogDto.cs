using System;

namespace BlogApi.DTO;

/// <summary>
/// DTO para representar un log de actividad del usuario, incluyendo su ID, acción realizada, fecha y detalles adicionales.
/// </summary>
public class LogDto
{
    /// <summary>
    /// ID del log.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// ID del usuario que realizó la acción.
    /// </summary>
    public int UsuarioId { get; set; }
    /// <summary>
    /// Acción realizada por el usuario (por ejemplo, "Inicio de sesión", "Creación de post", etc.).
    /// </summary>/
    public string Accion { get; set; } = string.Empty;
    /// <summary>
    /// Fecha y hora en que se realizó la acción.
    /// </summary>/
    public DateTime Fecha { get; set; }
    /// <summary>
    /// Detalles adicionales sobre la acción realizada (por ejemplo, IP del usuario, dispositivo utilizado, etc.).
    /// </summary>
    public string Detalles { get; set; } = string.Empty;
}
