using System;

namespace BlogApi.DTO;

/// <summary>
/// DTO para crear un nuevo permiso, contiene la clave única del permiso y una descripción opcional, la clave es un identificador único que se utiliza para asignar el permiso a los roles y verificar los permisos de los usuarios en el sistema, se recomienda utilizar claves descriptivas que reflejen la funcionalidad o acción que representa el permiso (ejemplo: "Usuarios.Editar", "Articulos.Crear", etc.), la descripción es un campo opcional que se puede utilizar para proporcionar información adicional sobre el permiso, aunque no es obligatorio, puede ser útil para la administración y gestión de permisos en el sistema (ejemplo: "Permiso para editar usuarios", "Permiso para crear artículos", etc.)
/// </summary>
public class CrearPermisoDto
{
    /// <summary>
    /// Clave única del permiso, es un campo obligatorio y debe ser único en el sistema, se recomienda utilizar claves descriptivas que reflejen la funcionalidad o acción que representa el permiso (ejemplo: "Usuarios.Editar", "Articulos.Crear", etc.)
    /// </summary>
    public string Clave { get; set; } = string.Empty;

    /// <summary>
    /// Descripción opcional del permiso, se puede utilizar para proporcionar información adicional sobre el permiso, aunque no es obligatorio, puede ser útil para la administración y gestión de permisos en el sistema (ejemplo: "Permiso para editar usuarios", "Permiso para crear artículos", etc.)
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;
}
