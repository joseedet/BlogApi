using BlogApi.Models;
using BlogApi.Repositories;

namespace BlogApi.Services;

public class UsuarioService : IUsuarioService
{
    /// <summary>
    ///     Repositorio de usuarios
    /// </summary>
    private readonly IUsuarioRepository _repo;

    /// <summary>
    /// Constructor de UsuarioService
    /// </summary>
    /// <param name="repo"></param>
    /// </summary>
    public UsuarioService(IUsuarioRepository repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Obtiene un usuario por su email
    /// </summary>
    /// <param name="email"></param>
    /// <returns>Usuario o null</returns>
    /// </summary>
    public async Task<Usuario?> GetByEmailAsync(string email) => await _repo.GetByEmailAsync(email);

    /// <summary>
    /// Crea un nuevo usuario
    /// </summary>
    public async Task<Usuario> CrearUsuarioAsync(Usuario usuario)
    {
        if (usuario.Rol == 0)
            usuario.Rol = RolUsuario.Suscriptor;
        await _repo.AddAsync(usuario);
        await _repo.SaveChangesAsync();
        return usuario;
    }
}
