using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.DTO;
using BlogApi.Models;

namespace BlogApi.Services.Interfaces;

public interface IPostService
{
    /// <summary>
    /// Obtiene todos los posts
    /// </summary>
    /// <returns>IEnumerable<Post></returns>
    Task<IEnumerable<Post>> GetAllAsync();

    /// <summary>
    /// Obtiene un post por su id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Post</returns>
    Task<Post?> GetByIdAsync(int id);

    /// <summary>
    /// Elimina un post por su id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="usuarioId"></param>
    /// <param name="puedeEditarTodo"></param>
    /// <returns>bool</returns>
    Task<bool> DeleteAsync(int id, int usuarioId, bool puedeEditarTodo);

    /// <summary>
    /// Crea un nuevo post con tags asociados
    /// </summary>
    /// <param name="post"></param>
    /// <param name="tagIds"></param>
    /// <param name="usuarioId"></param>
    /// <returns>Post</returns>
    Task<Post> CreateAsync(Post post, List<int> tagIds, int usuarioId);

    //Task<bool> UpdateAsync(int id, Post post, List<int> tagIds);

    /// <summary>
    /// Actualiza un post existente
    /// </summary>
    /// <param name="id"></param>
    /// <param name="post"></param>
    /// <param name="tagIds"></param>
    /// <param name="puedeEditarTodo"></param>
    /// <param name="usuarioId"></param>
    /// <returns>bool</returns>
    Task<bool> UpdateAsync(
        int id,
        Post post,
        List<int> tagIds,
        int usuarioId,
        bool puedeEditarTodo
    );

    /// <summary>
    /// Obtiene los posts paginados
    /// </summary>
    /// <param name="pagina"></param>
    /// <param name="tamano"></param>
    /// <returns>PaginationDto&lt;Post&gt;</returns>
    Task<PaginationDto<Post>> GetPagedAsync(int pagina, int tamano);

    /// <summary>
    /// Obtiene un post por su slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns>Post</returns>
    Task<Post?> GetBySlugAsync(string slug);

    /// <summary>
    /// Métodos nuevos para busquedas y filtrados
    /// </summary>
    /// <param name="texto"></param>
    /// <returns>IEnumerable&lt;Post&gt;</returns>
    Task<IEnumerable<Post>> SearchAsync(string texto);

    /// <summary>
    /// Paginación de la busqueda
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
    Task<IEnumerable<Post>> GetByAutorAsync(int usuarioId);

    /// <summary>
    /// Obtiene los posts de un autor específico mediante su nombre
    /// </summary>
    /// <param name="nombre"></param>
    /// <returns>IEnumerable&lt;Post&gt;></returns>
    Task<IEnumerable<Post>> GetByAutorNombreAsync(string nombre);

    /// <summary>
    /// Obtiene los posts paginados con paginación por cursor
    /// </summary>
    /// <param name="after"></param>
    /// <param name="limit"></param>
    /// <returns>CursorPaginationDto&lt;Post&gt;></returns>
    Task<CursorPaginationDto<Post>> GetCursorPagedAsync(int? after, int limit);

    /// <summary>
    /// Contador de visitas
    /// </summary>
    /// <param name="postId"></param>
    /// <returns></returns>
    Task IncrementViewCountAsync(int postId);
    /// <summary>
    /// Cuenta todos los post publicados
    /// </summary>
    /// <param name="count"></param>
    /// <returns>List&lt;tPostDto&gt;</returns>
    Task<List<PostDto>> GetMostViewedAsync(int count);

    /// <summary>
    /// Cuenta el numero de comentarios de un post
    /// </summary>
    /// <param name="count"></param>
    /// <returns>List&lt;Post&gt;</returns>
    Task<List<PostDto>> GetMostCommentedAsync(int count);

    /// <summary>
    /// Obtiene los pots relacionados
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="count"></param>
    /// <returns>List&lt;PostDto&gt;</returns>
    Task<List<PostDto>> GetRelatedPostsAsync(int postId, int count);
}
