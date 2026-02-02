using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repositories;

/// <summary>
///     Repositorio para manejar operaciones de comentarios
/// </summary>
public class ComentarioRepository : GenericRepository<Comentario>, IComentarioRepository
{
    /// <summary>
    ///    Constructor de ComentarioRepository
    /// </summary>
    /// <param name="context"></param>
    /// </summary>
    public ComentarioRepository(BlogDbContext context)
        : base(context) { }

    /// <summary>
    ///   Obtiene los comentarios asociados a un post específico
    /// </summary>
    /// <param name="postId"></param>
    /// <returns>IEnumerable<Comentario></returns>
    /// </summary>
    public async Task<IEnumerable<Comentario>> GetByPostIdAsync(int postId)
    {
        return await _dbSet
            .Where(c => c.PostId == postId && c.ComentarioPadreId == null)
            .Include(c => c.Respuestas)
            .OrderByDescending(c => c.FechaCreacion)
            .ToListAsync();
    }

    /// <summary>
    ///   Obtiene las respuestas de un comentario específico
    /// </summary>
    /// <param name="comentarioId"></param>
    /// <returns>IEnumerable<Comentario></returns>
    /// </summary>
    public async Task<IEnumerable<Comentario>> GetRespuestasAsync(int comentarioId)
    {
        return await _dbSet
            .Where(c => c.ComentarioPadreId == comentarioId)
            .Include(c => c.Respuestas)
            .OrderBy(c => c.FechaCreacion)
            .ToListAsync();
    }

    /// <summary>
    ///   Obtiene un comentario por su ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Comentario?</returns>
    /// </summary>
    public async Task<Comentario?> GetByIdAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Id == id);
    }

    public IQueryable<Comentario> Query()
    {
        return _context.Comentarios.AsQueryable();
    }

    /// <summary>
    /// Cuenta
    /// </summary>
    /// <returns></returns>
    public Task<int> CountAsync()
    {
        return _context.Comentarios.CountAsync();
    }

    /// <summary>
    /// Comentarios recientes
    /// </summary>
    /// <param name="limit"></param>
    /// <returns>List&lt;Comentario&gt;</returns>
    public Task<List<Comentario>> GetRecentComentariosAsync(int limit)
    {
        return _context
            .Comentarios.OrderByDescending(c => c.FechaCreacion)
            .Take(limit)
            .ToListAsync();
    }
}
