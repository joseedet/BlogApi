using System.Net;
using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BlogApi.Services.Security;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly ISanitizerService _sanitizer;
    private readonly ILogger<PostService> _logger;

    public PostService(
        IPostRepository postRepository,
        ISanitizerService sanitizer,
        ILogger<PostService> logger
    )
    {
        _postRepository = postRepository;
        _sanitizer = sanitizer;
        _logger = logger;
    }

    // ------------------------------------------------------------
    // CREATE
    // ------------------------------------------------------------
    public async Task<Post> CreateAsync(Post post, List<int> tagIds)
    {
        // Sanitización
        post.Titulo = _sanitizer.SanitizePlainText(post.Titulo);
        post.Contenido = _sanitizer.SanitizeMarkdown(post.Contenido);

        // Validación anti-XSS burda
        if (ContienePatronPeligroso(post.Titulo) || ContienePatronPeligroso(post.Contenido))
        {
            _logger.LogWarning("Intento XSS detectado en creación de post: {Titulo}", post.Titulo);
            throw new ArgumentException("El contenido contiene elementos no permitidos.");
        }

        // Asignar tags
        post.Tags = tagIds.Select(id => new Tag { Id = id }).ToList();

        await _postRepository.AddAsync(post);
        return post;
    }

    // ------------------------------------------------------------
    // GET ALL
    // ------------------------------------------------------------
    public async Task<IEnumerable<Post>> GetAllAsync()
    {
        var posts = await _postRepository.GetAllAsync();

        foreach (var post in posts)
        {
            post.Titulo = WebUtility.HtmlEncode(post.Titulo);
        }

        return posts;
    }

    // ------------------------------------------------------------
    // GET BY ID
    // ------------------------------------------------------------
    public async Task<Post?> GetByIdAsync(int id)
    {
        return await _postRepository.GetPostCompletoAsync(id);
    }

    // ------------------------------------------------------------
    // DELETE
    // ------------------------------------------------------------
    public async Task<bool> DeleteAsync(int id)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null)
            return false;

        _postRepository.Remove(post);
        await _postRepository.SaveChangesAsync();
        return true;
    }

    // ------------------------------------------------------------
    // UPDATE
    // ------------------------------------------------------------
    public async Task<bool> UpdateAsync(int id, Post post, List<int> tagIds, bool puedeEditarTodo)
    {
        var existente = await _postRepository.GetPostCompletoAsync(id);
        if (existente == null)
            return false;

        // Sanitización
        existente.Titulo = _sanitizer.SanitizePlainText(post.Titulo);
        existente.Contenido = _sanitizer.SanitizeMarkdown(post.Contenido);

        // Validación anti-XSS
        if (
            ContienePatronPeligroso(existente.Titulo)
            || ContienePatronPeligroso(existente.Contenido)
        )
        {
            _logger.LogWarning(
                "Intento XSS detectado en actualización de post: {Titulo}",
                existente.Titulo
            );
            throw new ArgumentException("El contenido contiene elementos no permitidos.");
        }

        // Tags
        existente.Tags = tagIds.Select(id => new Tag { Id = id }).ToList();

        _postRepository.Update(existente);
        await _postRepository.SaveChangesAsync();
        return true;
    }

    // ------------------------------------------------------------
    // PAGINACIÓN
    // ------------------------------------------------------------
    public async Task<PaginationDto<Post>> GetPagedAsync(int pagina, int tamano)
    {
        return await _postRepository.GetPagedAsync(pagina, tamano);
    }

    // ------------------------------------------------------------
    // CURSOR PAGINATION
    // ------------------------------------------------------------
    public async Task<CursorPaginationDto<Post>> GetCursorPagedAsync(int? after, int limit)
    {
        return await _postRepository.GetCursorPagedAsync(after, limit);
    }

    // ------------------------------------------------------------
    // SLUG
    // ------------------------------------------------------------
    public async Task<Post?> GetBySlugAsync(string slug)
    {
        return await _postRepository.GetBySlugAsync(slug);
    }

    // ------------------------------------------------------------
    // SEARCH
    // ------------------------------------------------------------
    public async Task<IEnumerable<Post>> SearchAsync(string texto)
    {
        return await _postRepository.SearchAsync(texto);
    }

    public async Task<PaginationDto<Post>> SearchPagedAsync(string texto, int pagina, int tamano)
    {
        return await _postRepository.SearchPagedAsync(texto, pagina, tamano);
    }

    // ------------------------------------------------------------
    // CATEGORÍAS
    // ------------------------------------------------------------
    public async Task<IEnumerable<Post>> GetByCategoriaAsync(int categoriaId)
    {
        return await _postRepository.GetByCategoriaAsync(categoriaId);
    }

    public async Task<IEnumerable<Post>> GetByCategoriaSlugAsync(string slug)
    {
        return await _postRepository.GetByCategoriaSlugAsync(slug);
    }

    // ------------------------------------------------------------
    // TAGS
    // ------------------------------------------------------------
    public async Task<IEnumerable<Post>> GetByTagAsync(int tagId)
    {
        return await _postRepository.GetByTagAsync(tagId);
    }

    public async Task<IEnumerable<Post>> GetByTagNombreAsync(string nombre)
    {
        return await _postRepository.GetByTagNombreAsync(nombre);
    }

    // ------------------------------------------------------------
    // AUTOR
    // ------------------------------------------------------------
    public async Task<IEnumerable<Post>> GetByAutorAsync(int usuarioId)
    {
        return await _postRepository.GetByAutorAsync(usuarioId);
    }

    public async Task<IEnumerable<Post>> GetByAutorNombreAsync(string nombre)
    {
        return await _postRepository.GetByAutorNombreAsync(nombre);
    }

    // ------------------------------------------------------------
    // ANTI-XSS BÁSICO
    // ------------------------------------------------------------
    private bool ContienePatronPeligroso(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return false;

        var lower = input.ToLowerInvariant();

        return lower.Contains("<script")
            || lower.Contains("javascript:")
            || lower.Contains("onerror=")
            || lower.Contains("onload=")
            || lower.Contains("<iframe")
            || lower.Contains("<svg");
    }
}
