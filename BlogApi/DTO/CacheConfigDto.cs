using System;

namespace BlogApi.DTO;

/// <summary>
/// DTO para representar la configuración de caché de la aplicación, incluyendo los tiempos de expiración para diferentes tipos de datos. Este DTO se utiliza para transferir la configuración de caché entre el cliente y el servidor, permitiendo que el cliente pueda actualizar los tiempos de expiración según sea necesario.
/// </summary>
public class CacheConfigDto
{
    /// <summary>
    /// Tiempo de expiración en segundos para los posts en caché. Se establece en 60 segundos por defecto, lo que significa que los posts se actualizarán cada minuto.
    /// </summary>
    public int ExpiracionPostsSegundos { get; set; }

    /// <summary>
    /// Tiempo de expiración en segundos para los comentarios en caché. Se establece en 30 segundos por defecto, lo que significa que los comentarios se actualizarán cada 30 segundos.
    /// </summary>
    public int ExpiracionComentariosSegundos { get; set; }

    /// <summary>
    /// Tiempo de expiración en segundos para el dashboard en caché. Se establece en 120 segundos por defecto, lo que significa que el dashboard se actualizará cada 2 minutos.
    /// </summary>
    public int ExpiracionDashboardSegundos { get; set; }

    /// <summary> Tiempo de expiración en segundos para los roles en caché. Se establece en 300 segundos por defecto, lo que significa que los roles se actualizarán cada 5 minutos.
    /// </summary>
    public int ExpiracionRolesSegundos { get; set; }

    /// <summary> Tiempo de expiración en segundos para los permisos en caché. Se establece en 300 segundos por defecto, lo que significa que los permisos se actualizarán cada 5 minutos.
    /// </summary>
    public int ExpiracionPermisosSegundos { get; set; }

    /// <summary> Tiempo de expiración en segundos para los usuarios en caché. Se establece en 60 segundos por defecto, lo que significa que los usuarios se actualizarán cada minuto.
    /// </summary>
    public int ExpiracionUsuariosSegundos { get; set; }

    /// <summary> Tiempo de expiración en segundos para el listado completo de posts en caché. Se establece en 60 segundos por defecto, lo que significa que el listado de posts se actualizará cada minuto.
    /// </summary>
    public int ExpiracionPostsListadoSegundos { get; set; }

    /// <summary>
    /// Tiempo de expiración en segundos para el slug de post en caché.Se establece en 60 segundo por defecto.
    /// </summary>
    public int ExpiracionPostPorSlugSegundos { get; set; }

    /// <summary>
    /// Tiempo de expiración en segundos para post por categoria slug en cache. Se establece en 60 segundos por defecto, lo que significa que slug se actualizará cada minuto.
    /// </summary>
    public int ExpiracionPostsPorCategoriaSlugSegundos { get; set; }

    /// <summary>
    /// Tiempo de expiración en segundos para post por categoria en cache. Se establece en 60 segundos por defecto, lo que significa que slug se actualizará cada minuto.
    /// </summary>
    public int ExpiracionPostsPorCategoriaIdSegundos { get; set; }

    /// <summary>
    /// Tiempo de expiración en segundos para post por tag en cache. Se establece en 60 segundos por defecto, lo que significa que slug se actualizará cada minuto.
    /// </summary>
    public int ExpiracionPostsPorTagIdSegundos { get; set; }
}
