using System;
using BlogApi.DTO;
using BlogApi.Models;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Interfaz para el servicio de permisos, que maneja operaciones relacionadas con los permisos de usuario en el sistema
/// </summary>
public interface IPermisoService
{
    /// <summary>
    /// Obtiene todos los permisos disponibles en la base de datos, ordenados por nombre
    /// </summary>
    /// <returns>List&lt;Permiso&gt;</returns>
    Task<List<Permiso>> GetAllAsync();

    /// <summary>
    /// Crea un nuevo permiso en la base de datos utilizando los datos proporcionados en el DTO CrearPermisoDto, devuelve el permiso creado con su ID asignado, este método se utiliza para agregar nuevos permisos al sistema desde la interfaz de administración o panel de control (solo accesible para usuarios con permisos de administración)
    /// </summary>
    Task<Permiso?> CrearPermisoAsync(CrearPermisoDto dto);

    /// <summary>
    /// Edita un permiso existente en la base de datos utilizando los datos proporcionados en el DTO EditarPermisoDto, devuelve un booleano indicando si la operación fue exitosa o no, este método se utiliza para actualizar los permisos existentes en el sistema desde la interfaz de administración o panel de control (solo accesible para usuarios con permisos de administración)
    /// </summary>
    /// <param name="permisoId"></param>
    /// <param name="dto"></param>
    /// <returns>Booleano indicando si la operación fue exitosa o no</returns>
    Task<bool> EditarPermisoAsync(int permisoId, EditarPermisoDto dto);

    /// <summary>
    /// Elimina un permiso existente en la base de datos utilizando el ID del permiso a eliminar, devuelve un booleano indicando si la operación fue exitosa o no, este método se utiliza para eliminar permisos existentes en el sistema desde la interfaz de administración o panel de control (solo accesible para usuarios con permisos de administración)
    /// </summary>
    /// <param name="permisoId"></param>
    /// <returns>Booleano indicando si la operación fue exitosa o no</returns>
    Task<bool> EliminarPermisoAsync(int permisoId);
}
