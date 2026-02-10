using System;

namespace BlogApi.DTO;

/// <summary>
/// DTO para representar el uso de un rol, incluyendo su nombre y la cantidad de usuarios que lo utilizan.
/// </summary>
public class RolUsoDto
{
    /// <summary>
    /// Nombre del rol.
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Cantidad de usuarios que utilizan este rol.
    /// </summary>
    public int CantidadUsuarios { get; set; }
}
