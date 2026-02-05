using System;
using BlogApi.Models;

namespace BlogApi.Repositories.Interfaces;

/// <summary>
/// Interfaz repositorio de Page
/// </summary>
public interface IPageRepository
{
    /// <summary>
    /// Obtenemos pagina por Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Page</returns>
    Task<Page> ObtenerPorIdAsync(int id);

    /// <summary>
    /// Obtener pagina por slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns>Page</returns>
    Task<Page> ObtenerPorSlugAsync(string slug);

    /// <summary>
    /// Obtener todas las páginas.
    /// </summary>
    /// <returns>List&lt;Page&gt;</returns>
    Task<List<Page>> ObtenerTodasAsync();

    /// <summary>
    /// Crea página
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    Task CrearAsync(Page page);

    /// <summary>
    /// Actualiza una página
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    Task ActualizarAsync(Page page);

    /// <summary>
    /// Elimina una página
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    Task EliminarAsync(Page page);

    /// <summary>
    /// Guarda la versión de la pagina
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    Task GuardarVersionAsync(PageVersion version);
    /// <summary>
    /// Obtiene versiones de página
    /// </summary>
    /// <param name="pageId"></param>
    /// <returns>List&lt;PageVersion&gt;</returns>

    Task<List<PageVersion>> ObtenerVersionesAsync(int pageId);

    /// <summary>
    /// Obtiene versión por ID
    /// </summary>
    /// <param name="versionId"></param>
    /// <returns>PageVersion</returns>
    Task<PageVersion> ObtenerVersionPorIdAsync(int versionId);

    
   
}
