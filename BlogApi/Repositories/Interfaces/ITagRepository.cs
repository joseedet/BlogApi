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

    /// <summary>
    /// ¿Existe este Slug?
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    Task<bool> SlugExistsAsync(string slug);

    /// <summary>
    /// Contador de post por tag
    /// </summary>
    /// <param name="tagId"></param>
    /// <returns>int</returns>
    Task<int> CountPostsAsync(int tagId);

    /// <summary>
    /// Contador de post  por tag
    /// <returns>int</returns>
    Task<int> CountAsync();

}
