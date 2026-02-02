using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Utils.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repositories;

/// <summary>
/// Repositorio específico para la entidad Tag
/// </summary>
public class TagRepository : GenericRepository<Tag>, ITagRepository
{
    /// <summary>
    /// Constructor
    /// /// </summary>
    /// <param name="context"></param>
    public TagRepository(BlogDbContext context)
        : base(context) { }

    /// <summary>
    /// Obtiene una lista de etiquetas por sus IDs
    /// </summary>
    /// <param name="ids"></param>
    /// <returns>List&lt;Tag&gt;</returns>
    public async Task<List<Tag>> GetByIdsAsync(List<int> ids)
    {
        return await _context.Tags.Where(t => ids.Contains(t.Id)).ToListAsync();
    }

    /// <summary>
    /// ¿Existe este Slug?
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    public Task<bool> SlugExistsAsync(string slug)
    {
        return _dbSet.AnyAsync(x => x.Slug == slug);
    }

    /// <summary>
    /// Contador de post por tag
    /// </summary>
    /// <param name="tagId"></param>
    /// <returns>int</returns>
    public Task<int> CountPostsAsync(int tagId)
    {
        return _context
            .Posts.Where(p => p.Estado == PostEstado.Publicado)
            .Where(p => p.Tags.Any(t => t.Id == tagId))
            .CountAsync();
    }

    /// <summary>
    /// Cuenta el numero de post por tag
    /// <returns>Entero</returns>
    public Task<int> CountAsync()
    {
        return _context.Tags.CountAsync();
    }
}
