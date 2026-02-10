using System;

namespace BlogApi.DTO;

/// <summary>
/// DTO para crear un nuevo rol, contiene el nombre del rol, una descripción opcional y una lista de IDs de permisos que se asignarán al rol al momento de su creación (opcional, se pueden asignar permisos después de crear el rol utilizando el endpoint correspondiente)
/// </summary>
public class CrearRolDto
{
    /// <summary>
    /// Nombre del rol a crear, es un campo obligatorio y debe ser único en el sistema, se recomienda utilizar nombres descriptivos que reflejen las responsabilidades o permisos asociados al rol (ejemplo: "Administrador", "Editor", "Usuario", etc.)
    /// </summary>
    public string Nombre { get; set; } = string.Empty;
    /// <summary>
    /// Descripción opcional del rol, se puede utilizar para proporcionar información adicional sobre las responsabilidades o permisos asociados al rol, aunque no es obligatorio, puede ser útil para la administración y gestión de roles en el sistema (ejemplo: "Rol con acceso completo a todas las funcionalidades del sistema", "Rol con permisos limitados para editar contenido", etc.)
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;
    /// <summary>
    /// Lista de IDs de permisos que se asignarán al rol al momento de su creación, este campo es opcional y permite asignar permisos específicos al rol desde el momento en que se crea, si no se proporcionan permisos en este campo, el rol se creará sin permisos asignados y se podrán asignar posteriormente utilizando el endpoint correspondiente para editar los permisos de un rol (ejemplo: [1, 2, 3] donde cada número representa el ID de un permiso específico en el sistema)
    /// </summary>
    // Opcional: crear el rol con permisos ya asignados 
    public List<int> PermisosIds { get; set; } = new();
}
