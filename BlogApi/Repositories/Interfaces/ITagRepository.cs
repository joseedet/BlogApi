using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Models;
using BlogApi.Repositories.Interfaces;

namespace BlogApi.Repositories;

/// <summary>
/// Repositorio específico para la entidad Tag
/// </summary>
public interface ITagRepository : IGenericRepository<Tag>
{
    /// <summary>
    /// Obtiene una lista de etiquetas por sus IDs
    /// </summary>
    /// <param name="ids"></param>
    /// <returns>List&lt;Tag&gt;</returns>
    Task<List<Tag>> GetByIdsAsync(List<int> ids);
}
