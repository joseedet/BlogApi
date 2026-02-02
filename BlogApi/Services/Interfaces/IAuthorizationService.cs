using System.Security.Claims;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Interfaz del servicio de autorización
/// </summary>
public interface IAuthorizationServiceBlog
{
    /// <summary>
    /// Es admin
    /// </summary>
    /// <param name="user"></param>
    /// <returns>Devuelve verdadero si lo es, en caso contrario falso</returns>
    bool EsAdmin(ClaimsPrincipal user);

    /// <summary>
    /// Es Editor
    /// </summary>
    /// <param name="user"></param>
    /// <returns>Devuelve verdadero si lo es, en caso contrario falso</returns>
    bool EsEditor(ClaimsPrincipal user);

    /// <summary>
    /// Es Usuario
    /// </summary>
    /// <param name="user"></param>
    /// <returns>Devuelve verdadero si lo es, en cso contrario falso</returns>
    bool EsUsuario(ClaimsPrincipal user);

    /// <summary>
    /// Obtener el Id del usuario
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    int GetUserId(ClaimsPrincipal user);

    /// <summary>
    /// Es propietario
    /// </summary>
    /// <param name="recursoUserId"></param>
    /// <param name="user"></param>
    /// <returns>Devuelve verdadero si lo es, en caso contrario falso</returns>
    bool EsPropietario(int recursoUserId, ClaimsPrincipal user);
}
