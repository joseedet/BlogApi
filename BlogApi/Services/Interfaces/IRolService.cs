using System;
using BlogApi.Models;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Interfaz para el servicio de roles
/// </summary>
public interface IRolService
{
    /// <summary>
    /// Obtiene todos los roles disponibles
    /// </summary>
    /// <returns>List&lt;Rol&gt;</returns>
    Task<List<Rol>> GetAllAsync();
}
