using System;

namespace BlogApi.Models;

/// <summary>
/// Modelo para representar la configuración de caché de la aplicación, incluyendo los tiempos de expiración para diferentes tipos de datos.
/// </summary>
public class CacheConfig
{
    /// <summary>
    /// Identificador único para la configuración de caché. Se establece en 1 por defecto, ya que se espera que solo haya una configuración de caché en la base de datos.
    /// </summary>
    public int Id { get; set; } = 1;

    /// <summary>
    /// Tiempo de expiración en segundos para los posts en caché. Se establece en 60 segundos por defecto, lo que significa que los posts se actualizarán cada minuto.
    /// </summary>
    public int ExpiracionPostsSegundos { get; set; } = 60;

    /// <summary>
    /// Tiempo de expiración en segundos para los comentarios en caché. Se establece en 30 segundos por defecto, lo que significa que los comentarios se actualizarán cada 30 segundos.
    /// </summary>
    public int ExpiracionComentariosSegundos { get; set; } = 30;

    /// <summary>
    /// Tiempo de expiración en segundos para el dashboard en caché. Se establece en 120 segundos por defecto, lo que significa que el dashboard se actualizará cada 2 minutos.
    /// </summary>
    public int ExpiracionDashboardSegundos { get; set; } = 120;

    /// <summary>
    /// Tiempo de expiración en segundos para los roles en caché. Se establece en 300 segundos por defecto, lo que significa que los roles se actualizarán cada 5 minutos.
    /// </summary>
    public int ExpiracionRolesSegundos { get; set; } = 300;

    /// <summary>
    /// Tiempo de expiración en segundos para los permisos en caché. Se establece en 300 segundos por defecto, lo que significa que los permisos se actualizarán cada 5 minutos.
    /// </summary>
    public int ExpiracionPermisosSegundos { get; set; } = 300;

    /// <summary>
    /// Tiempo de expiración en segundos para los usuarios en caché. Se establece en 60 segundos por defecto, lo que significa que los usuarios se actualizarán cada minuto.
    /// </summary>
    public int ExpiracionUsuariosSegundos { get; set; } = 60;

    /// <summary>
    /// Tiempo de expiración en segundos para el listado completo de posts en caché. Se establece en 60 segundos por defecto, lo que significa que el listado de posts se actualizará cada minuto.
    /// </summary>/
    public int ExpiracionPostsListadoSegundos { get; set; } = 60;

    /// <summary>
    /// Tiempo de expiración en segundos para post por slug en cache. Se establece en 60 segundos por defecto, lo que significa que slug se actualizará cada minuto.
    /// </summary>
    public int ExpiracionPostPorSlugSegundos { get; set; } = 60;

    /// <summary>
    /// Tiempo de expiración en segundos para post por categoria slug en cache. Se establece en 60 segundos por defecto, lo que significa que slug se actualizará cada minuto.
    /// </summary>
    public int ExpiracionPostsPorCategoriaSlugSegundos { get; set; } = 60;


    

}
