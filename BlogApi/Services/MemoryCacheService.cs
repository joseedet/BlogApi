using System;
using System.Text.Json;
using BlogApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace BlogApi.Services;

/// <summary>
/// Implementación de ICacheService utilizando IDistributedCache para almacenamiento en memoria.
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<MemoryCacheService> _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="logger"></param>
    public MemoryCacheService(IDistributedCache cache, ILogger<MemoryCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }
    /// <summary>
    /// Obtiene un valor de la caché o lo establece si no existe, con una expiración relativa.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="obtenerDatos"></param>
    /// <param name="expiracion"></param>
    /// <returns></returns>
    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> obtenerDatos,
        TimeSpan expiracion
    )
    {
        var cached = await _cache.GetStringAsync(key);
        if (cached != null)
        {
            _logger.LogInformation("CACHE HIT (Memory): {Key}", key);
            return JsonSerializer.Deserialize<T>(cached)!;
        }
        _logger.LogInformation("CACHE MISS (Memory): {Key}", key);
        var valor = await obtenerDatos();
        var json = JsonSerializer.Serialize(valor);
        await _cache.SetStringAsync(
            key,
            json,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiracion }
        );
        _logger.LogInformation(
            "CACHE SET (Memory): {Key} expira en {Exp}s",
            key,
            expiracion.TotalSeconds
        );
        return valor;
    }
    /// <summary>
    /// Elimina un valor de la caché por su clave.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>Tarea de finalización</returns>
    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
        _logger.LogWarning("CACHE INVALIDATE (Memory): {Key}", key);
    }
}
