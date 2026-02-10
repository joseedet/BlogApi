using System;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Interfaz para el servicio de caché, que define los métodos para obtener o establecer valores en caché y para eliminar valores de caché. Este servicio se utiliza para optimizar el rendimiento de la aplicación al almacenar temporalmente datos que se consultan con frecuencia, evitando consultas repetitivas a la base de datos o a servicios externos. La implementación de esta interfaz se encargará de gestionar la lógica de almacenamiento en caché, incluyendo la expiración de los datos según los tiempos configurados en la aplicación.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Obtiene un valor de caché para la clave especificada. Si el valor no existe o ha expirado, se ejecuta la función `obtenerDatos` para obtener los datos, se almacena en caché con el tiempo de expiración especificado y luego se devuelve el valor obtenido. Este método es útil para optimizar el rendimiento de la aplicación al evitar consultas repetitivas a la base de datos o a servicios externos, almacenando los resultados en caché durante un período de tiempo determinado.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="obtenerDatos"></param>
    /// <param name="expiracion"></param>
    /// <returns></returns>
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> obtenerDatos, TimeSpan expiracion);

    /// <summary>
    /// Elimina un valor de caché para la clave especificada. Este método se utiliza para invalidar o eliminar datos almacenados en caché cuando ya no son válidos o cuando se desea forzar la actualización de los datos en la próxima solicitud. Al eliminar un valor de caché, se asegura que la próxima vez que se solicite ese dato, se obtendrá una versión actualizada en lugar de una versión obsoleta almacenada en caché.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>Una tarea que representa la operación de eliminación.</returns>
    Task RemoveAsync(string key);
}
