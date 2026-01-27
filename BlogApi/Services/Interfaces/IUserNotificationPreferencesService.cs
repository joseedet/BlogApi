using System;
using BlogApi.DTO;
using BlogApi.Models;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Interfaz preferencias notificación usuario
/// </summary>
public interface IUserNotificationPreferencesService
{
    /// <summary>
    /// Obtención del Id del usuario
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>UserNotificationPreferences</returns>
    Task<UserNotificationPreferences> GetByUserIdAsync(int userId);

    /// <summary>
    /// Actualización del las preferencias de notificación del usuario.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task UpdateAsync(int userId, UserNotificationPreferencesDto dto);
}
