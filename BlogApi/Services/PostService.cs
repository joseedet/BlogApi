using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services.Interfaces;
using BlogApi.Utils;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

/// <summary>
///     Servicio para manejar la lógica de posts
/// </summary>
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
    /// Repositorio de categorías
    /// </summary>
    private readonly ICategoriaRepository _categoriaRepository;

    /// <summary>
    ///     Servicio de sanitización de entradas
    /// </summary>
    private readonly ISanitizerService _sanitizerService;

    /// <summary>
    /// Servicio de notificaciones
    /// </summary>
    private readonly INotificacionService _notificationService;

    /// <summary>
    /// Repositorio de categorías
    /// </summary>
    /// <summary>
    /// Constructor de PostService
    /// </summary>
    /// <param name="repo"></param>
    /// <param name="tagRepo"></param>
    /// <param name="categoriaRepository"></param>
    /// <param name="sanitizerService"></param>
    /// <param name="notificationService"></param>
    public PostService(
        IPostRepository repo,
        ITagRepository tagRepo,
        ICategoriaRepository categoriaRepository,
        ISanitizerService sanitizerService,
        INotificacionService notificationService
    )
    {
        _repo = repo;
        _tagRepo = tagRepo;
        _categoriaRepository = categoriaRepository;
        _sanitizerService = sanitizerService;
        _notificationService = notificationService;
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
        ValidarEntrada(post, tagIds);
        await ValidarCategoriaAsync(post.CategoriaId);
        await ValidarTagsAsync(tagIds);
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
    /// <param name="usuarioId"></param>
    /// <returns>bool</returns>
    public async Task<bool> UpdateAsync(
        int id,
        Post post,
        List<int> tagIds,
        int usuarioId,
        bool puedeEditarTodo
    )
    {
        ValidarEntrada(post, tagIds);
        await ValidarCategoriaAsync(post.CategoriaId);
        await ValidarTagsAsync(tagIds);

        var existing = await _repo
            .Query()
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (existing == null)
            return false;

        // 🔥 VALIDACIÓN DE PERMISOS
        if (!puedeEditarTodo && existing.UsuarioId != usuarioId)
            return false;

        // Actualizar campos básicos
        existing.Titulo = post.Titulo;
        existing.Contenido = post.Contenido;
        existing.CategoriaId = post.CategoriaId;
        existing.FechaActualizacion = DateTime.UtcNow;

        // ⚠️ Nunca actualices UsuarioId desde el body
        // existing.UsuarioId = post.UsuarioId;  ❌
        // El usuario del post NO debe cambiar

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
    /// <param name="usuarioId"></param>
    /// <param name="puedeEditarTodo"></param>
    /// <returns>bool</returns>
    public async Task<bool> DeleteAsync(int id, int usuarioId, bool puedeEditarTodo)
    {
        var existing = await _repo.GetByIdAsync(id);

        if (existing == null)
            return false;

        // Validación de permisos
        if (!puedeEditarTodo && existing.UsuarioId != usuarioId)
            return false;

        _repo.Remove(existing);
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
            Items = datos,
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
            Items = datos,
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

        return new CursorPaginationDto<Post> { Items = datos, NextCursor = nextCursor };
    }

    /// <summary>
    /// Valida la entrada para crear o actualizar un post
    /// </summary>
    /// <param name="post"></param>
    /// <param name="tagIds"></param>
    /// <exception cref="ArgumentException"></exception>
    private void ValidarEntrada(Post post, List<int> tagIds)
    {
        if (post == null)
            throw new ArgumentException("El post no puede ser nulo");

        if (string.IsNullOrWhiteSpace(post.Titulo))
            throw new ArgumentException("El título es obligatorio");

        if (string.IsNullOrWhiteSpace(post.Contenido))
            throw new ArgumentException("El contenido es obligatorio");

        if (post.CategoriaId <= 0)
            throw new ArgumentException("La categoría es inválida");

        if (tagIds == null)
            throw new ArgumentException("La lista de tags no puede ser nula");

        if (tagIds.Any(id => id <= 0))
            throw new ArgumentException("Todos los tags deben tener un ID válido");
    }

    /// <summary>
    /// Valida que la categoría exista
    /// </summary>
    /// <param name="categoriaId"></param>
    /// <exception cref="ArgumentException"></exception>
    private async Task ValidarCategoriaAsync(int categoriaId)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(categoriaId);

        if (categoria == null)
            throw new ArgumentException("La categoría no existe");
    }

    /// <summary>
    /// Valida que los tags existan y sean válidos
    /// </summary>
    /// <param name="tagIds"></param>
    /// <exception cref="ArgumentException"></exception>
    private async Task ValidarTagsAsync(List<int> tagIds)
    {
        if (tagIds == null)
            throw new ArgumentException("La lista de tags no puede ser nula");

        if (tagIds.Count == 0)
            throw new ArgumentException("Debes seleccionar al menos un tag");

        if (tagIds.Any(id => id <= 0))
            throw new ArgumentException("Todos los tags deben tener un ID válido");

        if (tagIds.Distinct().Count() != tagIds.Count)
            throw new ArgumentException("La lista de tags contiene duplicados");

        var tagsExistentes = await _tagRepo.GetByIdsAsync(tagIds);

        if (tagsExistentes.Count != tagIds.Count)
            throw new ArgumentException("Uno o más tags no existen");
    }

    /// <summary>
    /// Valida los permisos para editar un post
    /// </summary>
    /// <param name="post"></param>
    /// <param name="usuarioId"></param>
    /// <param name="puedeEditar"></param>
    /// <exception cref="UnauthorizedAccessException"></exception>
    private void ValidarPermisos(Post post, int usuarioId, bool puedeEditar)
    {
        if (!puedeEditar)
            throw new UnauthorizedAccessException("No tienes permisos para editar posts");

        // Si no es admin, debe ser el autor del post
        if (post.UsuarioId != usuarioId)
            throw new UnauthorizedAccessException("No puedes editar posts de otros usuarios");
    }

    /// <summary>
    /// Elimina un post por su id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>bool</returns>
    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Actualiza un post junto con sus etiquetas
    /// </summary>
    /// <param name="id"></param>
    /// <param name="post"></param>
    /// <param name="tagIds"></param>
    /// <param name="usuarioId"></param>
    /// <param name="puedeEditarTodo"></param>
    /// <returns>bool</returns>
    public Task<bool> UpdateAsync(int id, Post post, List<int> tagIds, bool puedeEditarTodo)
    {
        throw new NotImplementedException();
    }
}
