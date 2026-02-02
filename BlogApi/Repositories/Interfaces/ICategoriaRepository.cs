using BlogApi.Models;

namespace BlogApi.Repositories.Interfaces;

/// <summary>
/// Interfaz del repositorio de categoria.
/// </summary>
public interface ICategoriaRepository : IGenericRepository<Categoria>
{
    /// <summary>
    /// ¿Existe este Slug?
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    Task<bool> SlugExistsAsync(string slug);

    /// <summary>
    /// Contador de post por categoría
    /// </summary>
    /// <param name="categoriaId"></param>
    /// <returns>int</returns>
    Task<int> CountPostsAsync(int categoriaId);

    /// <summary>
    /// Contador de post por categoría
    /// </summary>
    /// <returns>int</returns>
    Task<int> CountAsync();
}
