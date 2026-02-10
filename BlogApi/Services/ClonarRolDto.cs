using System;

namespace BlogApi.Services;

/// <summary>
/// DTO para clonar un rol, contiene el nuevo nombre y la nueva descripción del rol a crear, utilizado para crear un nuevo rol basado en un rol existente, copiando sus permisos y asignaciones de usuarios, este DTO se utiliza en el controlador de roles para clonar un rol específico del sistema
/// </summary>
public class ClonarRolDto
{
    /// <summary>
    /// Nuevo nombre del rol a crear, utilizado para identificar el nuevo rol que se creará al clonar un rol existente, debe ser un nombre único y descriptivo del rol que representa, este campo es obligatorio para crear el nuevo rol basado en el rol existente
    /// </summary>
    public string NuevoNombre { get; set; } = string.Empty;

    /// <summary>
    /// Nueva descripción del rol a crear, utilizada para proporcionar una descripción legible del nuevo rol que se creará al clonar un rol existente, no es obligatoria pero puede ayudar a entender el propósito del nuevo rol al mostrarlo en la interfaz de administración de roles y permisos
    /// </summary>
    public string NuevaDescripcion { get; set; } = string.Empty;
}
