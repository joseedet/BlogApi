using System;
using BlogApi.Data;
using BlogApi.Models;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Interfaz para los ajustes de notificación
/// </summary>/
public interface INotificationSettingsService
{
    /// <summary>
    /// Cargamos los ajustes activos
    /// </summary>
    /// <returns></returns>
    Task<NotificationSettings> GetActiveAsync();

    /// <summary>
    /// Actualizamos los ajustes de notificación
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    Task UpdateAsync(NotificationSettings settings);
}
