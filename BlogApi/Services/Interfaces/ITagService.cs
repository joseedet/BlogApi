using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Models;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Interfaz
/// </summary>
public interface ITagService
{
    /// <summary>
    /// Dame todos los tag
    /// </summary>
    /// <returns>IEnumerable Tag</returns>
    Task<IEnumerable<Tag>> GetAllAsync();

    /// <summary>
    /// Obtener por Tag por id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Tag</returns>
    Task<Tag?> GetByIdAsync(int id);

    /// <summary>
    /// Crear tag
    /// </summary>
    /// <param name="tag"></param>
    /// <returns>Tag</returns>
    Task<Tag> CreateAsync(Tag tag);

    /// <summary>
    /// Actualiza el tag
    /// </summary>
    /// <param name="id"></param>
    /// <param name="tag"></param>
    /// <returns>Verdadero se se ha actualizado en caso contrario falso</returns>
    Task<bool> UpdateAsync(int id, Tag tag);

    /// <summary>
    /// Elimina el tag
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Verdadero si se ha eliminado en caso contrario falso</returns>
    Task<bool> DeleteAsync(int id);
    //IQueryable<Tag> Query();
}
