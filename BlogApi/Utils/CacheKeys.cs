using System;

namespace BlogApi.Utils;

/// <summary>
/// Clase estática que define las claves de caché utilizadas en la aplicación para almacenar y recuperar datos de manera eficiente.
/// </summary>
public class CacheKeys
{
    /// <summary>
    /// Clave para almacenar el listado completo de posts en caché.
    /// </summary>
    public const string PostsListado = "posts_listado";

    /// <summary>
    /// Clave para almacenar el listado de posts recientes en caché.
    /// </summary>
    public const string PostsRecientes = "posts_recientes";
    
    /// <summary>
    /// Genera una clave de caché específica para un post dado su ID, lo que permite almacenar y recuperar información de ese post de manera eficiente.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Una cadena que representa la clave de caché para un post específico.</returns>

    public static string PostPorId(int id) => $"post_{id}";
}
