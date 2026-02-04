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
    /// <returns></returns>
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
}
