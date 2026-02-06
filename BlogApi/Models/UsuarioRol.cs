using System;

namespace BlogApi.Models;

/// <summary>
/// Usuario Rol
/// </summary>
public class UsuarioRol
{
    /// <summary>
    /// Identificación del usuario
    /// </summary>
    public int UsuarioId { get; set; }

    /// <summary>
    /// Usuario
    /// </summary>
    public Usuario Usuario { get; set; } = null!;

    /// <summary>
    /// Identificador del rol
    /// </summary>
    public int RolId { get; set; }
    /// <summary>
    /// Rol
    /// </summary>
    public Rol Rol { get; set; } = null!;


}
