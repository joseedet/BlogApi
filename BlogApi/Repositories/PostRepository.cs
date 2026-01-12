using BlogApi.Data;
using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repositories;

/// <summary>
/// Repositorio específico para Posts
/// </summary>
public class PostRepository : GenericRepository<Post>, IPostRepository
{
    /// <summary>
    /// Constructor del repositorio de posts
    /// </summary>
    /// <param name="context"></param>
    public PostRepository(BlogDbContext context)
        : base(context) { }

    /// <summary>
    /// Obtiene un post completo por su id, incluyendo relaciones
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Post</returns>
    // ------------------------------------------------------------
    // POST COMPLETO (incluye relaciones principales)
    // ------------------------------------------------------------
    public async Task<Post?> GetPostCompletoAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Obtiene un post por su slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns>Post</returns>
    // ------------------------------------------------------------
    // SLUG
    // ------------------------------------------------------------
    public async Task<Post?> GetBySlugAsync(string slug)
    {
        return await _dbSet
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    /// <summary>
    /// Obtiene los posts paginados
    /// </summary>
    /// <param name="pagina"></param>
    /// <param name="tamano"></param>
    /// <returns>PaginationDto&lt;Post&gt;</returns>
    // ------------------------------------------------------------
    // PAGINACIÓN NORMAL
    // ------------------------------------------------------------
    public async Task<PaginationDto<Post>> GetPagedAsync(int pagina, int tamano)
    {
        var query = _dbSet
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .OrderByDescending(p => p.FechaCreacion);

        var total = await query.CountAsync();
        var items = await query.Skip((pagina - 1) * tamano).Take(tamano).ToListAsync();

        return new PaginationDto<Post>(items, total, pagina, tamano);
    }

    /// <summary>
    /// Obtiene los posts paginados con cursor
    /// </summary>
    /// <param name="after"></param>
    /// <param name="limit"></param>
    /// <returns>CursorPaginationDto&lt;Post&gt;</returns>
    // ------------------------------------------------------------
    // PAGINACIÓN POR CURSOR
    // ------------------------------------------------------------
    public async Task<CursorPaginationDto<Post>> GetCursorPagedAsync(int? after, int limit)
    {
        var query = _dbSet
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .OrderByDescending(p => p.Id)
            .AsQueryable();

        if (after.HasValue)
            query = query.Where(p => p.Id < after.Value);

        var items = await query.Take(limit).ToListAsync();

        int? nextCursor = items.Count == limit ? items.Last().Id : null;

        return new CursorPaginationDto<Post>(items, nextCursor);
    }

    /// <summary>
    /// Busca posts que coincidan con el texto dado
    /// </summary>
    /// <param name="texto"></param>
    /// <returns>IEnumerable&lt;Post&gt;</returns>
    // ------------------------------------------------------------
    // BÚSQUEDAS
    // ------------------------------------------------------------
    public async Task<IEnumerable<Post>> SearchAsync(string texto)
    {
        texto = texto.ToLower();

        return await _dbSet
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .Where(p => p.Titulo.ToLower().Contains(texto) || p.Contenido.ToLower().Contains(texto))
            .ToListAsync();
    }

    /// <summary>
    /// Busca posts que coincidan con el texto dado y los devuelve paginados
    /// </summary>
    /// <param name="texto"></param>
    /// <param name="pagina"></param>
    /// <param name="tamano"></param>
    /// <returns>PaginationDto&lt;Post&gt;</returns>
    // ------------------------------------------------------------
    // BÚSQUEDAS PAGINADAS
    public async Task<PaginationDto<Post>> SearchPagedAsync(string texto, int pagina, int tamano)
    {
        texto = texto.ToLower();

        var query = _dbSet
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .Where(p =>
                p.Titulo.ToLower().Contains(texto) || p.Contenido.ToLower().Contains(texto)
            );

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.FechaCreacion)
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .ToListAsync();

        return new PaginationDto<Post>(items, total, pagina, tamano);
    }

    /// <summary>
    /// Obtiene los posts de una categoría específica
    /// </summary>
    /// <param name="categoriaId"></param>
    /// <returns>IEnumerable&lt;Post&gt;</returns>
    // ------------------------------------------------------------
    // CATEGORÍAS
    // ------------------------------------------------------------
    public async Task<IEnumerable<Post>> GetByCategoriaAsync(int categoriaId)
    {
        return await _dbSet
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .Where(p => p.CategoriaId == categoriaId)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene los posts de una categoría específica mediante su slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns>IEnumerable&lt;Post&gt;</returns>
    public async Task<IEnumerable<Post>> GetByCategoriaSlugAsync(string slug)
    {
        return await _dbSet
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .Where(p => p.Categoria.Slug == slug)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene los posts asociados a una etiqueta específica
    /// </summary>
    /// <param name="tagId"></param>
    /// <returns>IEnumerable&lt;Post&gt;</returns>
    // ------------------------------------------------------------
    // TAGS
    // ------------------------------------------------------------
    public async Task<IEnumerable<Post>> GetByTagAsync(int tagId)
    {
        return await _dbSet
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .Where(p => p.Tags.Any(t => t.Id == tagId))
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene los posts asociados a una etiqueta específica mediante su nombre
    /// </summary>
    /// <param name="nombre"></param>
    /// <returns>IEnumerable&lt;Post&gt;</returns>
    public async Task<IEnumerable<Post>> GetByTagNombreAsync(string nombre)
    {
        nombre = nombre.ToLower();

        return await _dbSet
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .Where(p => p.Tags.Any(t => t.Nombre.ToLower() == nombre))
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene los posts de un autor específico
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns>IEnumerable&lt;Post&gt;</returns>
    // ------------------------------------------------------------
    // AUTOR
    // ------------------------------------------------------------
    public async Task<IEnumerable<Post>> GetByAutorAsync(int usuarioId)
    {
        return await _dbSet
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .Where(p => p.UsuarioId == usuarioId)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene los posts de un autor específico mediante su nombre
    /// </summary>
    /// <param name="nombre"></param>
    /// <returns>IEnumerable&lt;Post&gt;</returns>
    public async Task<IEnumerable<Post>> GetByAutorNombreAsync(string nombre)
    {
        nombre = nombre.ToLower();

        return await _dbSet
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .Where(p => p.Usuario.Nombre.ToLower() == nombre)
            .ToListAsync();
    }
}
