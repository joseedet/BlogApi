using System;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace BlogApi.Services;

public class RedisCacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> obtenerDatos,
        TimeSpan expiracion
    )
    {
        var cached = await _cache.GetStringAsync(key);
        if (cached != null)
        {
            _logger.LogInformation("CACHE HIT (Redis): {Key}", key);
            return JsonSerializer.Deserialize<T>(cached)!;
        }
        _logger.LogInformation("CACHE MISS (Redis): {Key}", key);
        var valor = await obtenerDatos();
        var json = JsonSerializer.Serialize(valor);
        await _cache.SetStringAsync(
            key,
            json,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiracion }
        );
        _logger.LogInformation(
            "CACHE SET (Redis): {Key} expira en {Exp}s",
            key,
            expiracion.TotalSeconds
        );
        return valor;
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
        _logger.LogWarning("CACHE INVALIDATE (Redis): {Key}", key);
    }
}
