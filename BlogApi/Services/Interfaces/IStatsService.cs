using System;
using BlogApi.DTO;

namespace BlogApi.Services.Interfaces;
/// <summary>
/// Estadisticas
/// </summary>
public interface IStatsService
{
    /// <summary>
    /// Obtiene las estadísticas
    /// </summary>
    /// <returns></returns>

    Task<BlogStatsDto> GetEstadisticasAsync();

     /// <summary>
    /// Obtiene la actividad reciente.
    /// </summary>
    /// <returns></returns>
    Task<ActividadRecienteDto> GetActividadRecienteAsync(int limit = 10);
}
