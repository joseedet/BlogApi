using BlogApi.Data;
using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repositories;

public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(BlogDbContext context)
        : base(context) { }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        //return await context.Set<Usuario>().FirstOrDefaultAsync(u => u.Email == email);
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }
}
