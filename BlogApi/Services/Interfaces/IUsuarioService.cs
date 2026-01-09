using BlogApi.DTO;
using BlogApi.Models;

namespace BlogApi.Services.Interfaces;

public interface IUsuarioService
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario> CrearUsuarioAsync(Usuario usuario);
    Task<Usuario> RegistrarUsuarioAsync(RegistroDto dto);
}
