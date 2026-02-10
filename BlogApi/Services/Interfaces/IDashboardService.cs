using System;
using BlogApi.DTO;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Interfaz para el servicio de dashboard, que proporciona un método para obtener los datos del dashboard, incluyendo estadísticas de usuarios, roles, permisos y actividad reciente.
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Obtiene los datos del dashboard, incluyendo estadísticas de usuarios, roles, permisos y actividad reciente.
    /// </summary>
    /// <returns>Un objeto <see cref="DashboardDto"/> con los datos del dashboard.</returns>
    Task<DashboardDto> ObtenerDashboardAsync();
}
