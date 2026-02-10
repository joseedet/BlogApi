using System;
using BlogApi.Data;
using BlogApi.DTO;
using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

/// <summary>
/// Servicio para gestionar la configuración de caché de la aplicación, incluyendo tiempos de expiración para diferentes tipos de datos. Este servicio interactúa con la base de datos a través del contexto `BlogDbContext` para almacenar y recuperar la configuración de caché. Permite que el cliente pueda obtener la configuración actual y actualizarla según sea necesario, facilitando así la gestión eficiente de los datos en caché en toda la aplicación.
/// </summary>
public class CacheConfigService
{
    private readonly BlogDbContext _context;

    /// <summary>
    /// Constructor del servicio de configuración de caché, que recibe una instancia del contexto de la base de datos `BlogDbContext` a través de la inyección de dependencias. Este constructor se encarga de inicializar el contexto para que el servicio pueda interactuar con la base de datos y gestionar la configuración de caché de manera eficiente.
    /// </summary>
    /// <param name="context"></param>
    public CacheConfigService(BlogDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene la configuración de caché actual de la aplicación, incluyendo los tiempos de expiración para diferentes tipos de datos. Este método se utiliza para mostrar la configuración actual al cliente, permitiendo que el cliente pueda ver los tiempos de expiración actuales antes de realizar cualquier actualización.
    /// </summary>
    /// <returns>La configuración de caché actual.</returns>
    public async Task<CacheConfig> ObtenerConfigAsync()
    {
        return await _context.CacheConfig.FirstAsync();
    }

    /// <summary>
    /// Actualiza la configuración de caché de la aplicación con los nuevos tiempos de expiración proporcionados en el DTO. Este método se utiliza para permitir que el cliente actualice los tiempos de expiración para diferentes tipos de datos en caché, lo que puede ayudar a mejorar el rendimiento de la aplicación al ajustar los tiempos de actualización según las necesidades del cliente.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>Una tarea que representa la operación de actualización.</returns>
    public async Task ActualizarConfigAsync(CacheConfigDto dto)
    {
        var config = await _context.CacheConfig.FirstAsync();
        config.ExpiracionPostsSegundos = dto.ExpiracionPostsSegundos;
        config.ExpiracionComentariosSegundos = dto.ExpiracionComentariosSegundos;
        config.ExpiracionDashboardSegundos = dto.ExpiracionDashboardSegundos;
        config.ExpiracionRolesSegundos = dto.ExpiracionRolesSegundos;
        config.ExpiracionPermisosSegundos = dto.ExpiracionPermisosSegundos;
        config.ExpiracionUsuariosSegundos = dto.ExpiracionUsuariosSegundos;
        await _context.SaveChangesAsync();
    }
}
