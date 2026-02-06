using System;

namespace BlogApi.Models;

/// <summary>
/// Rol permiso
/// </summary>
public class RolPermiso
{
    /// <summary>
    /// Identificador de rol
    /// </summary>
    public int RolId { get; set; }

    /// <summary>
    /// Rol
    /// </summary>
    public Rol Rol { get; set; } = null!;

    /// <summary>
    /// Identificador permiso
    /// </summary>
    public int PermisoId { get; set; }

    /// <summary>
    /// Permiso
    /// </summary>
    public Permiso Permiso { get; set; } = null!;
}
