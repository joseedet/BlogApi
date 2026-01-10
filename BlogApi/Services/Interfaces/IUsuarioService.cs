using BlogApi.DTO;
using BlogApi.Models;

namespace BlogApi.Services.Interfaces;

/// <summary>
///
/// </summary>
public interface IUsuarioService
{
    /// <summary>
    /// Obtiene un usuario por su email
    /// </summary>
    /// <param name="email"></param>
    /// <returns>Usuario</returns>
    Task<Usuario?> GetByEmailAsync(string email);

    /// <summary>
    /// Crea un nuevo usuario
    /// </summary>
    /// <param name="usuario"></param>
    /// <returns>Usuario</returns>
    Task<Usuario> CrearUsuarioAsync(Usuario usuario);

    /// <summary>
    /// Registra un nuevo usuario
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>Usuario</returns>
    Task<Usuario> RegistrarUsuarioAsync(RegistroDto dto);

    /// <summary>
    /// Verifica el email del usuario
    /// </summary>
    /// <param name="token"></param>
    /// <returns>Verdadero si se verificó correctamente, falso en caso contrario</returns>
    Task<bool> VerificarEmailAsync(string token);
}
