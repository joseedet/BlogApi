using System;
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
}
