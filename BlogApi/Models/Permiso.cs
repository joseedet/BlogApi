using System;

namespace BlogApi.Models;

/// <summary>
/// Permiso
/// </summary>
public class Permiso
{
    /// <summary>
    /// Identificador
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Clave
    /// </summary>
    public string Clave { get; set; } = string.Empty; // "Posts.Editar" 

    /// <summary>
    /// Descripción
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;
    /// <summary>
    /// Rol Permisos
    /// </summary>
    public List<RolPermiso> RolPermisos { get; set; } = new();
}
