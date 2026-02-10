using System;

namespace BlogApi.DTO;

/// <summary>
/// DTO para editar los permisos de un rol, contiene una lista de IDs de permisos que se asignarán al rol
/// </summary>
public class EditarPermisosRolDto
{
    /// <summary>
    /// Lista de IDs de permisos que se asignarán al rol
    /// </summary>
    public List<int> PermisosIds { get; set; } = new();
}
