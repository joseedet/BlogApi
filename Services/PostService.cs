using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Utils;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

public class PostService : IPostService
{
    /// <summary>
    /// Repositorio de posts
    /// </summary>
    private readonly IPostRepository _repo;

    /// <summary>
    /// Repositorio de tags
    /// </summary>
    private readonly ITagRepository _tagRepo;

    /// <summary>
    /// Constructor de PostService
    /// </summary>
    /// <param name="repo"></param>
    /// <param name="tagRepo"></param>
    public PostService(IPostRepository repo, ITagRepository tagRepo)
    {
        _repo = repo;
        _tagRepo = tagRepo;
    }

    /// <summary>
    /// Obtiene todos los posts
    /// </summary>
    /// <returns>IEnumerable<Post></returns>
    public async Task<IEnumerable<Post>> GetAllAsync()
    {
        return await _repo
            .Query()
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .Include(p => p.Comentarios)
                .ThenInclude(c => c.Usuario)
            .Include(p => p.Comentarios)
                .ThenInclude(c => c.Respuestas)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene un post por su id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Post</returns>
    public async Task<Post?> GetByIdAsync(int id)
    {
        return await _repo
            .Query()
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .Include(p => p.Comentarios)
                .ThenInclude(c => c.Usuario)
            .Include(p => p.Comentarios)
                .ThenInclude(c => c.Respuestas)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Crea un nuevo post con tags asociados
    /// </summary>
    /// <param name="post"></param>
    /// <param name="tagIds"></param>
    /// <returns>Post</returns>
    public async Task<Post> CreateAsync(Post post, List<int> tagIds)
    {
        // Generar slug base
        var baseSlug = SlugHelper.GenerateSlug(post.Titulo);

        // Asegurar que sea único
        var slug = baseSlug;
        int contador = 1;
        while (await _repo.Query().AnyAsync(p => p.Slug == slug))
        {
            slug = $"{baseSlug}-{contador}";
            contador++;
        }
        post.Slug = slug;

        // Cargar tags desde la BD
        var tags = await _tagRepo.Query().Where(t => tagIds.Contains(t.Id)).ToListAsync();

        post.Tags = tags;

        await _repo.AddAsync(post);
        await _repo.SaveChangesAsync();

        return post;
    }

    /// <summary>
    /// Actualiza un post existente
    /// </summary>
    /// <param name="id"></param>
    /// <param name="post"></param>
    /// <param name="tagIds"></param>
    /// <param name="puedeEditarTodo"></param>
    /// <returns>bool</returns>
    public async Task<bool> UpdateAsync(int id, Post post, List<int> tagIds, bool puedeEditarTodo)
    {
        var existing = await _repo
            .Query()
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (existing == null)
            return false;

        // 🔥 VALIDACIÓN DE PERMISOS
        // Autor solo puede editar sus posts
        if (existing.UsuarioId != post.UsuarioId && !puedeEditarTodo)
            return false;

        // Actualizar campos básicos
        existing.Titulo = post.Titulo;
        existing.Contenido = post.Contenido;
        existing.CategoriaId = post.CategoriaId;
        existing.UsuarioId = post.UsuarioId;
        existing.FechaActualizacion = DateTime.UtcNow;

        // Actualizar tags
        var tags = await _tagRepo.Query().Where(t => tagIds.Contains(t.Id)).ToListAsync();

        existing.Tags.Clear();
        foreach (var tag in tags)
            existing.Tags.Add(tag);

        _repo.Update(existing);
        await _repo.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Elimina un post por su id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>bool</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        var post = await _repo.GetByIdAsync(id);
        if (post == null)
            return false;

        _repo.Remove(post);
        await _repo.SaveChangesAsync();

        return true;
    }

    // Implementación de paginación

    /// <summary>
    /// Obtiene los posts paginados
    /// </summary>
    /// <param name="pagina"></param>
    /// <param name="tamano"></param>
    /// <returns>PaginationDto<Post></returns>
    public async Task<PaginationDto<Post>> GetPagedAsync(int pagina, int tamano)
    {
        var query = _repo
            .Query()
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags);

        var total = await query.CountAsync();

        var datos = await query.Skip((pagina - 1) * tamano).Take(tamano).ToListAsync();

        return new PaginationDto<Post>
        {
            Pagina = pagina,
            Tamano = tamano,
            Total = total,
            Datos = datos,
        };
    }

    /// <summary>
    /// Obtiene un post por su slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns>Post</returns>
    public async Task<Post?> GetBySlugAsync(string slug)
    {
        return await _repo
            .Query()
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .Include(p => p.Comentarios)
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    /// <summary>
    /// Busca posts por texto
    /// </summary>
    /// <param name="texto"></param>
    /// <returns>IEnumerable<Post></returns>
    public async Task<IEnumerable<Post>> SearchAsync(string texto)
    {
        texto = texto.ToLower().Trim();

        return await _repo
            .Query()
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .Where(p =>
                p.Titulo.ToLower().Contains(texto)
                || p.Contenido.ToLower().Contains(texto)
                || p.Categoria.Nombre.ToLower().Contains(texto)
                || p.Usuario.Nombre.ToLower().Contains(texto)
                || p.Tags.Any(t => t.Nombre.ToLower().Contains(texto))
            )
            .ToListAsync();
    }

    /// <summary>
    /// Busca posts por texto con paginación
    /// </summary>
    /// <param name="texto"></param>
    /// <param name="pagina"></param>
    /// <param name="tamano"></param>
    /// <returns>PaginationDto<Post></returns>
    public async Task<PaginationDto<Post>> SearchPagedAsync(string texto, int pagina, int tamano)
    {
        texto = texto.ToLower().Trim();

        var query = _repo
            .Query()
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .Where(p =>
                p.Titulo.ToLower().Contains(texto)
                || p.Contenido.ToLower().Contains(texto)
                || p.Categoria.Nombre.ToLower().Contains(texto)
                || p.Usuario.Nombre.ToLower().Contains(texto)
                || p.Tags.Any(t => t.Nombre.ToLower().Contains(texto))
            );

        var total = await query.CountAsync();

        var datos = await query.Skip((pagina - 1) * tamano).Take(tamano).ToListAsync();

        return new PaginationDto<Post>
        {
            Pagina = pagina,
            Tamano = tamano,
            Total = total,
            Datos = datos,
        };
    }

    /// <summary>
    /// Obtiene los posts por categoría
    /// </summary>
    /// <param name="categoriaId"></param>
    /// <returns>IEnumerable<Post></returns>
    public async Task<IEnumerable<Post>> GetByCategoriaAsync(int categoriaId)
    {
        return await _repo
            .Query()
            .Where(p => p.CategoriaId == categoriaId)
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene los posts por categoría mediante su slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns>IEnumerable<Post></returns>
    public async Task<IEnumerable<Post>> GetByCategoriaSlugAsync(string slug)
    {
        return await _repo
            .Query()
            .Where(p => p.Categoria.Slug == slug)
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene los posts por tag
    /// </summary>
    /// <param name="tagId"></param>
    /// <returns>IEnumerable<Post></returns>
    public async Task<IEnumerable<Post>> GetByTagAsync(int tagId)
    {
        return await _repo
            .Query()
            .Where(p => p.Tags.Any(t => t.Id == tagId))
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene los posts por nombre de tag
    /// </summary>
    /// <param name="nombre"></param>
    /// <returns>IEnumerable<Post></returns>
    public async Task<IEnumerable<Post>> GetByTagNombreAsync(string nombre)
    {
        nombre = nombre.ToLower().Trim();

        return await _repo
            .Query()
            .Where(p => p.Tags.Any(t => t.Nombre.ToLower() == nombre))
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene los posts por autor
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns>IEnumerable<Post></returns>
    public async Task<IEnumerable<Post>> GetByAutorAsync(int usuarioId)
    {
        return await _repo
            .Query()
            .Where(p => p.UsuarioId == usuarioId)
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene los posts por nombre de autor
    /// </summary>
    /// <param name="nombre"></param>
    /// <returns>IEnumerable<Post></returns>
    public async Task<IEnumerable<Post>> GetByAutorNombreAsync(string nombre)
    {
        nombre = nombre.ToLower().Trim();

        return await _repo
            .Query()
            .Where(p => p.Usuario.Nombre.ToLower().Contains(nombre))
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene los posts con paginación por cursor
    /// </summary>
    /// <param name="after"></param>
    /// <param name="limit"></param>
    /// <returns>CursorPaginationDto<Post></returns>
    public async Task<CursorPaginationDto<Post>> GetCursorPagedAsync(int? after, int limit)
    {
        var query = _repo
            .Query()
            .Include(p => p.Categoria)
            .Include(p => p.Usuario)
            .Include(p => p.Tags)
            .OrderBy(p => p.Id);

        if (after.HasValue)
            query = (IOrderedQueryable<Post>)query.Where(p => p.Id > after.Value);

        var datos = await query
            .Take(limit + 1) // +1 para saber si hay más
            .ToListAsync();

        int? nextCursor = null;

        if (datos.Count > limit)
        {
            nextCursor = datos.Last().Id;
            datos.RemoveAt(datos.Count - 1); // quitar el extra
        }

        return new CursorPaginationDto<Post> { Datos = datos, NextCursor = nextCursor };
    }
}
