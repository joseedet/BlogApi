using BlogApi.Models;
using BlogApi.Repositories.Interfaces;

namespace BlogApi.Repositories;

public interface IUsuarioRepository : IGenericRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email);
}
