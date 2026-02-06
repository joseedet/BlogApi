using System;

namespace BlogApi.Models;

/// <summary>
/// Rol
/// </summary>
public class Rol
{
    /// <summary>
    /// Identificador
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre
    /// </summary>
    public string Nombre { get; set; } = string.Empty; // "Administrador"

    /// <summary>
    /// Descripción
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Usuarios
    /// </summary>
    public List<UsuarioRol> UsuarioRoles { get; set; } = new();
    /// <summary>
    /// Rol Permisos
    /// </summary>
    public List<RolPermiso> RolPermisos { get; set; } = new();
}
