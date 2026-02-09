using System;

namespace BlogApi.Models;

/// <summary>
/// Entidad que representa la relación entre un rol y un permiso, indicando qué permisos tiene cada rol en el sistema
/// </summary>
public class RolPermiso
{
    /// <summary>
    /// ID del rol al que se le asigna el permiso
    /// </summary>
    public int RolId { get; set; }

    /// <summary>
    /// Rol al que se le asigna el permiso
    /// </summary>
    public Rol Rol { get; set; }
    /// <summary>
    /// ID del permiso que se asigna al rol
    /// </summary>
    public int PermisoId { get; set; }
    /// <summary>
    /// Permiso que se asigna al rol
    /// </summary>
    public Permiso Permiso { get; set; }
}
