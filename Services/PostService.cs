using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Utils;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _repo;
    private readonly ITagRepository _tagRepo;

    public PostService(IPostRepository repo, ITagRepository tagRepo)
    {
        _repo = repo;
        _tagRepo = tagRepo;
    }

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

    /*public Task<Post> CreateAsync(Post post)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(int id, Post post)
    {
        throw new NotImplementedException();
    }*/
}
