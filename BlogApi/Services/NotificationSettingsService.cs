using System;
using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;
/// <summary>
/// Clase que implementa la interfaz de INotificationSettingsService
/// </summary>
public class NotificationSettingsService : INotificationSettingsService
{
    private readonly BlogDbContext _db;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="db"></param>
    public NotificationSettingsService(BlogDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Obtenemos la notificación/es que hay activa/s
    /// </summary>
    /// <returns>NotificationSettings</returns>
    public async Task<NotificationSettings> GetActiveAsync()
    {
        var settings = await _db.NotificationSettings.FirstOrDefaultAsync(s => s.Activo);
        // Si no existe, crear uno por defecto
        if (settings == null)
        {
            settings = new NotificationSettings();
            _db.NotificationSettings.Add(settings);
            await _db.SaveChangesAsync();
        }
        return settings;
    }

    /// <summary>
    /// Actualiza el ajuste de notificación
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    public async Task UpdateAsync(NotificationSettings settings)
    {
        _db.NotificationSettings.Update(settings);
        await _db.SaveChangesAsync();
    }
}
