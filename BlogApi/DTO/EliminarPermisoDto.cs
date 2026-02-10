using System;

namespace BlogApi.DTO;

/// <summary>
/// DTO para eliminar un permiso, contiene el ID del permiso a eliminar, utilizado para eliminar un permiso específico del sistema desde la interfaz de administración o panel de control (solo accesible para usuarios con permisos de administración)
/// </summary>
public class EliminarPermisoDto
{
    /// <summary>
    /// ID del permiso a eliminar, utilizado para identificar el permiso que se eliminará del sistema, debe ser un ID válido de un permiso existente en la base de datos, este DTO se utiliza en el controlador de permisos para eliminar un permiso específico del sistema
    /// </summary>
    public int Id { get; set; }
}
