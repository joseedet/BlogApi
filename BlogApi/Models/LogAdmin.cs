using System;

namespace BlogApi.Models;

/// <summary>
/// Modelo para registrar acciones administrativas como bloquear o desbloquear usuarios, eliminar contenido, etc.
/// </summary>
public class LogAdmin
{
    /// <summary>
    /// ID del log administrativo
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// ID del usuario administrador que realizó la acción
    /// </summary>
    public int UsuarioAdminId { get; set; } // quién hizo la acción 
    /// <summary>
    /// Información del usuario administrador que realizó la acción (opcional, para facilitar consultas sin necesidad de hacer join)
    /// </summary>
    public Usuario UsuarioAdmin { get; set; }
    /// <summary>
    /// Descripción de la acción realizada (ejemplo: "BloquearUsuario", "EliminarComentario", etc.)
    /// </summary>
    public string Accion { get; set; } = string.Empty; // ejemplo: "BloquearUsuario"
    /// <summary>
    /// ID del usuario afectado por la acción (si aplica, por ejemplo, el usuario que fue bloqueado o cuyo comentario fue eliminado)
     /// </summary>
    public int? UsuarioAfectadoId { get; set; } // a quién afectó
    /// <summary>
    /// Información del usuario afectado por la acción (opcional, para facilitar consultas sin necesidad de hacer join)
    /// </summary>
    public Usuario? UsuarioAfectado { get; set; }
    /// <summary>
    /// Fecha y hora en que se realizó la acción administrativa
    /// </summary>    

    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Detalles adicionales sobre la acción (opcional, por ejemplo, motivo del bloqueo, contenido del comentario eliminado, etc.)
    /// </summary>
    public string? Detalles { get; set; } // opcional
}
