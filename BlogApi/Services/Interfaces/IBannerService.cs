using System;
using BlogApi.DTO;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Interfaz de servicio de Banner
/// </summary>
public interface IBannerService
{
    /// <summary>
    /// Creación del Banner
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>BannerDto</returns>
    Task<BannerDto> CrearAsync(BannerCreateDto dto);

    /// <summary>
    /// Obtener por Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>BannerDto</returns>
    Task<BannerDto?> ObtenerPorIdAsync(int id);

    /// <summary>
    /// Obtener todos los banners
    /// </summary>
    /// <returns>IEnumerable BannerDto</returns>
    Task<IEnumerable<BannerDto>> ObtenerTodosAsync();

    /// <summary>
    /// Obtener banners activos
    /// </summary>
    /// <returns>IEnumerable BannerDto</returns>
    Task<IEnumerable<BannerDto>> ObtenerActivosAsync();

    /// <summary>
    /// Actualización del Banner
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns>BannerDto</returns>
    Task<BannerDto?> ActualizarAsync(int id, BannerUpdateDto dto);

    /// <summary>
    /// Elimina un banner
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Verdadero si se ha eliminado en caso contrario falso</returns>
    Task<bool> EliminarAsync(int id);
}
