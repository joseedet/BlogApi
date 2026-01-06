using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Models;

namespace BlogApi.Repositories;

public interface IComentarioRepository : IGenericRepository<Comentario>
{
    Task<IEnumerable<Comentario>> GetByPostIdAsync(int postId);
    Task<IEnumerable<Comentario>> GetRespuestasAsync(int comentarioId);
    Task<Comentario?> GetByIdAsync(int id);
}
