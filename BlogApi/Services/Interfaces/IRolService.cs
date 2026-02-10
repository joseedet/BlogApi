using System;
using BlogApi.Models;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Interfaz para el servicio de roles, define los métodos para actualizar los permisos de un rol, asignar un permiso a un rol y quitar un permiso de un rol
/// </summary>
public interface IRolService
{
    /// <summary>
    /// Actualiza los permisos de un rol específico
    /// </summary>
    /// <param name="rolId">ID del rol a actualizar</param>
    /// <param name="permisosIds">Lista de IDs de permisos a asignar al rol</param>
    /// <returns>True si se actualizó correctamente, false en caso contrario</returns>
    Task<bool> ActualizarPermisosRolAsync(int rolId, List<int> permisosIds);

    /// <summary>
    /// Asigna un permiso específico a un rol
    /// </summary>
    /// <param name="rolId"></param>
    /// <param name="permisoId"></param>
    /// <returns>True si se asignó correctamente, false en caso contrario</returns>
    Task<bool> AsignarPermisoAsync(int rolId, int permisoId);

    /// <summary>
    /// Quita un permiso específico de un rol
    /// </summary>
    /// <param name="rolId"></param>
    /// <param name="permisoId"></param>
    /// <returns>True si se quitó correctamente, false en caso contrario</returns>
    Task<bool> QuitarPermisoAsync(int rolId, int permisoId);

    /// <summary>
    /// Obtiene todos los roles disponibles en el sistema, devuelve una lista de objetos Rol que representan cada rol con sus propiedades, este método se utiliza para mostrar los roles disponibles en la interfaz de administración o panel de control (solo accesible para usuarios con permisos de administración)
    /// </summary>
    /// <returns>IEnumerable&lt;Rol&gt;</returns>
    Task<IEnumerable<Rol>> GetAllAsync();
}
