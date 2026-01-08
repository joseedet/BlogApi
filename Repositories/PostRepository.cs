using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repositories;

public class PostRepository : GenericRepository<Post>, IPostRepository
{
    public PostRepository(BlogDbContext context)
        : base(context) { }

    public async Task<Post?> GetPostCompletoAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Categoria)
            .Include(p => p.UsuarioId)
            //.Include(p => p.UsuarioModificacion)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
