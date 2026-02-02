using BlogApi.Models;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services.Interfaces;
using BlogApi.Utils;

namespace BlogApi.Services;

/// <summary>
/// Servicio de Categoría.
/// </summary>
public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _repo;

    /// <summary>
    /// Constructor de CategoriaService
    /// </summary>
    /// <param name="repo"></param>
    /// </summary>
    public CategoriaService(ICategoriaRepository repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Obtiene todas las categorías
    /// </summary>
    /// <returns>IEnumerable&lt;Categoria&gt;</returns>
    public async Task<IEnumerable<Categoria>> GetAllAsync() => await _repo.GetAllAsync();

    /// <summary>
    /// Obtiene una categoría por su ID </summary>
    /// <param name="id"></param>
    /// <returns>Categoría o null si no existe</returns>
    public async Task<Categoria?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    /// <summary>
    /// Crea una nueva categoría
    /// </summary>
    /// <param name="categoria"></param>
    /// <returns>Categoría creada</returns>
    public async Task<Categoria> CreateAsync(Categoria categoria)
    {
        categoria.Slug = await GenerateUniqueSlugAsync(categoria.Nombre);

        await _repo.AddAsync(categoria);
        await _repo.SaveChangesAsync();
        return categoria;
    }

    /// <summary>
    /// Actualiza una categoría existente
    /// </summary>
    /// <param name="id"></param>
    /// <param name="categoria"></param>
    /// <returns>True si se actualizó correctamente, false en caso contrario</returns>
    public async Task<bool> UpdateAsync(int id, Categoria categoria)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null)
            return false;
        existing.Nombre = categoria.Nombre;
        _repo.Update(existing);
        await _repo.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Elimina una categoría por su ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Devuelve verdadero si se eliminó correctamente, falso en caso contrario</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        var categoria = await _repo.GetByIdAsync(id);
        if (categoria == null)
            return false;
        _repo.Remove(categoria);
        await _repo.SaveChangesAsync();
        return true;
    }

    private async Task<string> GenerateUniqueSlugAsync(string nombre)
    {
        var baseSlug = SlugHelper.GenerateSlug(nombre);
        var slug = baseSlug;
        int counter = 1;

        while (await _repo.SlugExistsAsync(slug))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        return slug;
    }

    /// <summary>
    /// Contador de post por categoría
    /// </summary>
    /// <param name="categoriaId"></param>
    /// <returns>int</returns>
    public async Task<int> GetPostCountAsync(int categoriaId)
    {
        return await _repo.CountPostsAsync(categoriaId);
    }
}
