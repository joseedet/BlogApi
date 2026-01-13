using BlogApi.Data;
using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repositories;

/// <summary>
/// Repositorio específico para la entidad Tag
/// </summary>
public class TagRepository : GenericRepository<Tag>, ITagRepository
{
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
}
