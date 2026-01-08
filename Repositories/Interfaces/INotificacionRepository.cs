using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Models;

namespace BlogApi.Repositories;

/// <summary>
/// Interfaz para el repositorio de notificaciones
/// </summary>
public interface INotificacionRepository
{
    /// <summary>
    /// Crea una nueva notificación
    /// </summary>
    /// <param name="notificacion"></param>
    /// <returns></returns>
    
    Task CrearAsync(Notificacion notificacion);

    /// <summary>
    /// Obtiene una notificación por su ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Notificacion</returns>
    
    Task<Notificacion?> ObtenerPorIdAsync(int id);

    /// <summary>
    /// Obtiene todas las notificaciones de un usuario
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns>IEnumerable de notificaciones</returns>
    
    Task<IEnumerable<Notificacion>> ObtenerPorUsuarioAsync(int usuarioId);

    /// <summary>
    /// Marca una notificación como leída
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
   
    Task MarcarComoLeidaAsync(int id);
}
