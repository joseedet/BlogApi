using System;

namespace BlogApi.DTO;

/// <summary>
/// DTO para asignar un rol a un usuario desde el panel de administración
/// </summary>
public class AsignarRolDto
{
    /// <summary>
    /// ID del rol a asignar al usuario
    /// </summary>
    public int RolId { get; set; }
}
