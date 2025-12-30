using BlogApi.Models;

namespace BlogApi.Services;

public interface IUsuarioService
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario> CrearUsuarioAsync(Usuario usuario);
}
