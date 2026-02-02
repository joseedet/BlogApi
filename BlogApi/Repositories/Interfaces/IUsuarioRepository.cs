using BlogApi.Models;
using BlogApi.Repositories.Interfaces;

namespace BlogApi.Repositories;

/// <summary>
/// Interfaz del repositorio de usuario
/// </summary>
public interface IUsuarioRepository : IGenericRepository<Usuario>
{
    /// <summary>
    /// Usuario por email
    /// </summary>
    /// <param name="email"></param>
    /// <returns>Usuario</returns>
    Task<Usuario?> GetByEmailAsync(string email);

    /// <summary>
    /// Contador de post  por tag
    /// <returns>int</returns>
    Task<int> CountAsync();

    /// <summary>
    /// Obtiene la actividad reciente de usuarios
    /// </summary>
    /// <param name="limit"></param>
    /// <returns>List&lt;Usuario&gt;</returns>
    Task<List<Usuario>> GetRecentUsuariosAsync(int limit);
}
