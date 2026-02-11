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

    /// <summary>
    /// Genera una clave de caché para almacenar un listado de posts paginados, utilizando el número de página y el tamaño de página como parte de la clave para diferenciar entre diferentes conjuntos de datos paginados.
    /// </summary>
    /// <param name="pagina"></param>
    /// <param name="tamano"></param>
    /// <returns>Una cadena que representa la clave de caché para un listado de posts paginados.</returns>
    public static string PostsPaged(int pagina, int tamano) => $"posts_paged_{pagina}_{tamano}";

    /// <summary>
    /// Genera una clave de caché para almacenar un listado de posts paginados, utilizando el número de página y el tamaño de página como parte de la clave para diferenciar entre diferentes conjuntos de datos paginados. Este método tiene un nombre más descriptivo y adecuado para su propósito en comparación con el método anterior.
    /// </summary>
    /// <param name="pagina"></param>
    /// <param name="tamano"></param>
    /// <returns>Una cadena que representa la clave de caché para un listado de posts paginados.</returns>
    // Nuevo nombre correcto para paginación de posts
    public static string PostListed(int pagina, int tamano) => $"posts_listed_{pagina}_{tamano}";

    /// <summary>
    /// Genera un clave caché para al almacenar slug en post.
    /// </summary>
    /// <param name="slug"></param>
    /// <returns>Una cadena qu representa la clave de caché para un listado post por slug</returns>
    public static string PostBySlug(string slug) => $"post_slug_{slug}";

    /// <summary>
    /// Genera un clave caché para al almacenar post por categoria slug .
    /// </summary>
    /// <param name="slug"></param>
    /// <returns>Una cadena qu representa
    public static string PostsByCategoriaSlug(string slug) => $"posts_categoria_slug_{slug}";

    /// <summary>
    /// Genera un clave caché para al almacenar post categoria.
    /// </summary>
    /// <param name="slug"></param>
    /// <returns>Una cadena qu representa
    public static string PostsByCategoriaId(int categoriaId) => $"posts_categoria_id_{categoriaId}";
}
