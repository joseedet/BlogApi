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

    /// <summary>
    /// Existe Email
    /// </summary>
    /// <param name="email"></param>
    /// <param name="excludeUserId"></param>
    /// <returns>Verdadero si existe en caso contrario falso</returns>
    Task<bool> EmailExistsAsync(string email, int excludeUserId);

    /// <summary>
    /// Establece el salt
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="salt"></param>
    /// <returns></returns>
    Task EstablecerSaltVerificacionAsync(int userId, string salt);

    /// <summary>
    /// Marca el email como verificado
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task MarcarEmailVerificadoAsync(int userId);

    /// <summary>
    /// Obtiene usuario por TokenHash
    /// </summary>
    /// <param name="tokenHash"></param>
    /// <returns>Usuario?</returns>
    Task<Usuario?> ObtenerPorTokenHashAsync(string tokenHash);

    /// <summary>
    /// Bloquear usuario
    /// </summary>
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Verdadero si se bloqueó correctamente, falso en caso contrario</returns>
    Task<bool> BloquearAsync(int id);
    
    /// <summary>
    /// Desbloquear usuario
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Verdadero si se desbloqueó correctamente, falso en caso contrario</returns>
    Task<bool> DesbloquearAsync(int id);
}
