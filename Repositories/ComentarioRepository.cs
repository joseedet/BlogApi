using BlogApi.Data;
using BlogApi.Models;
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
            .OrderByDescending(c => c.Fecha)
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
            .OrderBy(c => c.Fecha)
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
}
