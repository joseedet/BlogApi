using System;

namespace BlogApi.DTO;

/// <summary>
/// DTO para editar un usuario desde el panel de administración
/// </summary>
public class EditarUsuarioAdminDto
{
    /// <summary>
    /// Nombre del usuario
    /// </summary>
    public string Nombre { get; set; } = string.Empty;
    /// <summary>
    /// Apellidos del usuario
    /// </summary>
    public string Apellidos { get; set; } = string.Empty;
    /// <summary>
    /// Email del usuario
    /// </summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Fecha de nacimiento del usuario
    /// </summary>
    public bool EstaBloqueado { get; set; } // Lista de roles que el admin quiere asignar
    /// <summary>
    /// Lista de IDs de roles que el admin quiere asignar al usuario
    /// </summary>
    public List<int> RolesIds { get; set; } = new();
}
