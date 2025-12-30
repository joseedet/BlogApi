using BlogApi.Models;
using BlogApi.Repositories;

namespace BlogApi.Services;

public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _repo;

    public CategoriaService(ICategoriaRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Categoria>> GetAllAsync() => await _repo.GetAllAsync();

    public async Task<Categoria?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    public async Task<Categoria> CreateAsync(Categoria categoria)
    {
        await _repo.AddAsync(categoria);
        await _repo.SaveChangesAsync();
        return categoria;
    }

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

    public async Task<bool> DeleteAsync(int id)
    {
        var categoria = await _repo.GetByIdAsync(id);
        if (categoria == null)
            return false;
        _repo.Remove(categoria);
        await _repo.SaveChangesAsync();
        return true;
    }
}
