using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Models;

namespace BlogApi.DTO;

/// <summary>
/// UsuarioDto
/// </summary>
public class UsuarioDto
{
    /// <summary>
    /// Identificador usuario dti
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Rol
    /// /// </summary>
    public RolUsuario Rol { get; set; }
}
