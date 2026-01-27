using System;
using BlogApi.Data;
using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

/// <summary>
/// Clase de servicio que implemta la interfaz IUserNotificationPrefencesService.
/// </summary>
public class UserNotificationPreferencesService : IUserNotificationPreferencesService
{
    private readonly BlogDbContext _db;

    /// <summary>
    /// Constructor de la clase
    /// </summary>
    /// <param name="db"></param>
    public UserNotificationPreferencesService(BlogDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Obtiene el Id del usuario
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>USerNotificationPreference</returns>
    public async Task<UserNotificationPreferences> GetByUserIdAsync(int userId)
    {
        var prefs = await _db.UserNotificationPreferences.FirstOrDefaultAsync(p =>
            p.UsuarioId == userId
        );
        if (prefs == null)
        {
            prefs = new UserNotificationPreferences { UsuarioId = userId };
            _db.UserNotificationPreferences.Add(prefs);
            await _db.SaveChangesAsync();
        }
        return prefs;
    }

    /// <summary>
    /// Actualiza UserNotificationPreferences
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task UpdateAsync(int userId, UserNotificationPreferencesDto dto)
    {
        var prefs = await GetByUserIdAsync(userId);
        prefs.ReceiveEmailNotifications = dto.ReceiveEmailNotifications;
        prefs.ReceiveInternalNotifications = dto.ReceiveInternalNotifications;
        prefs.NotifyOnComment = dto.NotifyOnComment;
        prefs.NotifyOnAdminMessage = dto.NotifyOnAdminMessage;
        prefs.NotifyOnSystemAlert = dto.NotifyOnSystemAlert;
        await _db.SaveChangesAsync();
    }
}
