using System;

namespace BlogApi.DTO;

/// <summary>
/// DTO para representar el uso de un permiso, incluyendo su clave y la cantidad de roles que lo utilizan.
/// </summary>
public class PermisoUsoDto
{
    /// <summary>
    /// Clave del permiso.
    /// </summary>
    public string Clave { get; set; } = string.Empty;

    /// <summary>
    /// Cantidad de roles que utilizan este permiso.
    /// </summary>
    public int CantidadRoles { get; set; }
}
