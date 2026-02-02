using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Models;

namespace BlogApi.Repositories.Interfaces;

/// <summary>
/// Interfaz del repositorio de comentario
/// </summary>
public interface IComentarioRepository : IGenericRepository<Comentario>
{
    /// <summary>
    /// Obtener comentarios sobre un post por id
    /// </summary>
    /// <param name="postId"></param>
    /// <returns>IEnumerable&lt;Comentario&gt;</returns>
    Task<IEnumerable<Comentario>> GetByPostIdAsync(int postId);

    /// <summary>
    /// Respuestas a un comentario
    /// </summary>
    /// <param name="comentarioId"></param>
    /// <returns>IEnumerable&lt;Comentario&gt;</returns>
    Task<IEnumerable<Comentario>> GetRespuestasAsync(int comentarioId);

    /// <summary>
    /// Obtener por Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>comentario</returns>
    Task<Comentario?> GetByIdAsync(int id);

    /// <summary>
    /// Consulta sobre comentario
    /// </summary>
    /// <returns>Comentario</returns>
    IQueryable<Comentario> Query();

    /// <summary>
    /// Contador de post  por tag
    /// <returns>int</returns>
    Task<int> CountAsync();

    /// <summary>
    /// Lista los comentarios recientes
    /// </summary>
    /// <param name="limit"></param>
    /// <returns>List&lt;Comentario&gt;</returns>
    Task<List<Comentario>> GetRecentComentariosAsync(int limit);
}
