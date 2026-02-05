using System;
using BlogApi.DTO;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Interfaz de servicio para Page.
/// </summary>
public interface IPageService
{
    /// <summary>
    /// Crear Page.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>PageDto</returns>
    Task<PageDto> CrearAsync(CrearPageDto dto);

    /// <summary>
    /// Actualizamos Page.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns>PageDto</returns>
    Task<PageDto> ActualizarAsync(int id, ActualizarPageDto dto);

    /// <summary>
    /// Obtiene Page por Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>PageDto</returns>
    Task<PageDto> ObtenerPorIdAsync(int id);

    /// <summary>
    /// Obtiene Page por slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns>PageDto</returns>
    Task<PageDto> ObtenerPorSlugAsync(string slug);

    /// <summary>
    /// Obtenemos todas las Page.
    /// </summary>
    /// <returns>List&lt;Page&gt;</returns>
    Task<List<PageListadoDto>> ObtenerTodasAsync();

    /// <summary>
    /// Elimina Page
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task EliminarAsync(int id);

    /// <summary>
    /// Obtenemos página de inicio
    /// </summary>
    /// <returns>PageDto</returns>
    Task<PageDto> ObtenerPaginaInicioAsync();

    /// <summary>
    /// Restaura la versión de la página
    /// </summary>
    /// <param name="versionId"></param>
    /// <returns>PageDto</returns>
    Task<PageDto> RestaurarVersionAsync(int versionId);

    /// <summary>
    /// Obtiene la versión de una página dada
    /// </summary>
    /// <param name="pageId"></param>
    /// <returns>List&lt;PageVersionDto&gt;</returns>
    Task<List<PageVersionDto>> ObtenerVersionesAsync(int pageId);

    /// <summary>
    /// Obtiene un versión específica de una página
    /// </summary>
    /// <param name="versionId"></param>
    /// <returns>PageVersionDto</returns>
    Task<PageVersionDto> ObtenerVersionPorIdAsync(int versionId);
}
