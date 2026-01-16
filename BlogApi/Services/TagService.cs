using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Services.Interfaces;

namespace BlogApi.Services;

/// <summary>
/// Servicio para gestionar tags
/// </summary>
public class TagService : ITagService
{
    /// <summary>
    /// Repositorio de tags
    /// </summary>
    private readonly ITagRepository _repo;

    /// <summary>
    /// Constructor de TagService
    /// </summary>
    /// <param name="repo"></param>
    /// </summary>
    public TagService(ITagRepository repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Obtiene todos los tags
    /// </summary>
    /// <returns>IEnumerable de tags</returns>
    /// </summary>
    public async Task<IEnumerable<Tag>> GetAllAsync() => await _repo.GetAllAsync();

    /// <summary>
    /// Obtiene un tag por su ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Tag o null</returns>
    /// </summary>
    public async Task<Tag?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    /// <summary>
    /// Crea un nuevo tag
    /// </summary>
    /// <param name="tag"></param>
    /// <returns>Tag creado</returns>
    /// </summary>
    public async Task<Tag> CreateAsync(Tag tag)
    {
        await _repo.AddAsync(tag);
        await _repo.SaveChangesAsync();
        return tag;
    }

    /// <summary>
    /// Actualiza un tag existente
    /// </summary>
    /// <param name="id"></param>
    /// <param name="tag"></param>
    /// <returns>bool</returns>
    /// </summary>
    public async Task<bool> UpdateAsync(int id, Tag tag)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null)
            return false;
        existing.Nombre = tag.Nombre;
        _repo.Update(existing);
        await _repo.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Elimina un tag por su ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>bool</returns>
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null)
            return false;
        _repo.Remove(existing);
        await _repo.SaveChangesAsync();
        return true;
    }
}
