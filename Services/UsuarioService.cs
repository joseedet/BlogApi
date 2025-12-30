using BlogApi.Models;
using BlogApi.Repositories;

namespace BlogApi.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repo;

    public UsuarioService(IUsuarioRepository repo)
    {
        _repo = repo;
    }

    public async Task<Usuario?> GetByEmailAsync(string email) => await _repo.GetByEmailAsync(email);

    public async Task<Usuario> CrearUsuarioAsync(Usuario usuario)
    {
        if (usuario.Rol == 0)
            usuario.Rol = RolUsuario.Suscriptor;
        await _repo.AddAsync(usuario);
        await _repo.SaveChangesAsync();
        return usuario;
    }
}
