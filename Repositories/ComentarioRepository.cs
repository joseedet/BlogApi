using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Data;
using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repositories;

public class ComentarioRepository : GenericRepository<Comentario>, IComentarioRepository
{
    public ComentarioRepository(BlogDbContext context)
        : base(context) { }

    public async Task<IEnumerable<Comentario>> GetByPostIdAsync(int postId)
    {
        return await _dbSet
            .Where(c => c.PostId == postId && c.ComentarioPadreId == null)
            .Include(c => c.Respuestas)
            .OrderByDescending(c => c.Fecha)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comentario>> GetRespuestasAsync(int comentarioId)
    {
        return await _dbSet
            .Where(c => c.ComentarioPadreId == comentarioId)
            .Include(c => c.Respuestas)
            .OrderBy(c => c.Fecha)
            .ToListAsync();
    }
    public async Task<Comentario?> GetByIdAsync(int id) { return await _dbSet.FirstOrDefaultAsync(c => c.Id == id); }
}
