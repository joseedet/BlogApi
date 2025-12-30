using BlogApi.Models;

namespace BlogApi.Services;

public interface ICategoriaService
{
    Task<IEnumerable<Categoria>> GetAllAsync();
    Task<Categoria?> GetByIdAsync(int id);
    Task<Categoria> CreateAsync(Categoria categoria);
    Task<bool> UpdateAsync(int id, Categoria categoria);
    Task<bool> DeleteAsync(int id);
}
