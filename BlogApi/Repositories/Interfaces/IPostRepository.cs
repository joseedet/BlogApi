using BlogApi.DTO;
using BlogApi.Models;

namespace BlogApi.Repositories.Interfaces;

/// <summary>
/// Repositorio específico para Posts
/// </summary>
public interface IPostRepository : IGenericRepository<Post>
{
    /// <summary>
    /// Obtiene un post completo por su id, incluyendo relaciones
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Post</returns>
    Task<Post?> GetPostCompletoAsync(int id);

    /// <summary>
    /// Obtiene un post por su slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns>Post</returns>
    // Slug
    Task<Post?> GetBySlugAsync(string slug);

    /// <summary>
    /// Obtiene los posts paginados
    /// </summary>
    /// <param name="pagina"></param>
    /// <param name="tamano"></param>
    /// <returns>PaginationDto&lt;Post&gt;</returns>
    // Paginación
    Task<PaginationDto<Post>> GetPagedAsync(int pagina, int tamano);

    /// <summary>
    /// Obtiene los posts paginados con cursor
    /// </summary>
    /// <param name="after"></param>
    /// <param name="limit"></param>
    /// <returns>CursorPaginationDto&lt;Post&gt;</returns>
    Task<CursorPaginationDto<Post>> GetCursorPagedAsync(int? after, int limit);

    /// <summary>
    /// Busca posts que coincidan con el texto dado
    /// </summary>
    /// <param name="texto"></param>
    /// <returns>IEnumerable&lt;Post&gt;</returns>
    // Búsquedas
    Task<IEnumerable<Post>> SearchAsync(string texto);

    /// <summary>
    /// Busca posts que coincidan con el texto dado y los devuelve paginados
    /// </summary>
    /// <param name="texto"></param>
    /// <param name="pagina"></param>
    /// <param name="tamano"></param>
    /// <returns>PaginationDto&lt;Post&gt;</returns>
    Task<PaginationDto<Post>> SearchPagedAsync(string texto, int pagina, int tamano);

    /// <summary>
    /// Obtiene los posts de una categoría específica
    /// </summary>
    /// <param name="categoriaId"></param>
    /// <returns>IEnumerable&lt;Post&gt;</returns>
    // Categorías
    Task<IEnumerable<Post>> GetByCategoriaAsync(int categoriaId);

    /// <summary>
    /// Obtiene los posts de una categoría específica mediante su slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns>IEnumerable&lt;Post&gt;</returns>
    Task<IEnumerable<Post>> GetByCategoriaSlugAsync(string slug);

    /// <summary>
    /// Obtiene los posts asociados a una etiqueta específica
    /// </summary>
    /// <param name="tagId"></param>
    /// <returns>IEnumerable&lt;Post&gt;</returns>
    // Tags
    Task<IEnumerable<Post>> GetByTagAsync(int tagId);

    /// <summary>
    /// Obtiene los posts asociados a una etiqueta específica mediante su nombre
    /// </summary>
    /// <param name="nombre"></param>
    /// <returns>IEnumerable&lt;Post&gt;</returns>
    Task<IEnumerable<Post>> GetByTagNombreAsync(string nombre);

    /// <summary>
    /// Obtiene los posts de un autor específico
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns>IEnumerable&lt;Post&gt;</returns>
    // Autor
    Task<IEnumerable<Post>> GetByAutorAsync(int usuarioId);

    /// <summary>
    /// Obtiene los posts de un autor específico mediante su nombre
    /// </summary>
    /// <param name="nombre"></param>
    /// <returns>IEnumerable&lt;Post&gt;</returns>
    Task<IEnumerable<Post>> GetByAutorNombreAsync(string nombre);

    /// <summary>
    /// Actualiza un post junto con sus etiquetas
    /// </summary>
    /// <param name="id"></param>
    /// <param name="post"></param>
    /// <param name="tagIds"></param>
    /// <param name="usuarioId"></param>
    /// <param name="puedeEditarTodo"></param>
    /// <returns>bool</returns>
    Task<bool> UpdateAsync(
        int id,
        Post post,
        List<int> tagIds,
        int usuarioId,
        bool puedeEditarTodo
    );
}
