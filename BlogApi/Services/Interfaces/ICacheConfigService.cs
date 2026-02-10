using System;
using BlogApi.DTO;
using BlogApi.Models;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Interfaz para el servicio de configuración de caché, que define los métodos para obtener y actualizar la configuración de caché de la aplicación. Este servicio se encarga de gestionar los tiempos de expiración para diferentes tipos de datos en caché, permitiendo que el cliente pueda actualizar estos tiempos según sea necesario. La implementación de esta interfaz se encargará de interactuar con la base de datos para almacenar y recuperar la configuración de caché.
/// </summary>
public interface ICacheConfigService
{
    /// <summary>
    /// Obtiene la configuración de caché actual de la aplicación, incluyendo los tiempos de expiración para diferentes tipos de datos. Este método se utiliza para mostrar la configuración actual al cliente, permitiendo que el cliente pueda ver los tiempos de expiración actuales antes de realizar cualquier actualización.
    /// </summary>
    /// <returns>La configuración de caché actual.</returns>
    Task<CacheConfig> ObtenerConfigAsync();

    /// <summary>
    /// Actualiza la configuración de caché de la aplicación con los nuevos tiempos de expiración proporcionados en el DTO. Este método se utiliza para permitir que el cliente actualice los tiempos de expiración para diferentes tipos de datos en caché, lo que puede ayudar a mejorar el rendimiento de la aplicación al ajustar los tiempos de actualización según las necesidades del cliente.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>Una tarea que representa la operación de actualización.</returns>
    Task ActualizarConfigAsync(CacheConfigDto dto);
}
