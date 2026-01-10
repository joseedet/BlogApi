using BlogApi.Models;

namespace BlogApi.DTO;

/// <summary>
/// Resultado del intento de login
/// </summary>
public class LoginResult
{
    /// <summary>
    /// Indica si el login fue exitoso
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensaje de error en caso de fallo
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Usuario autenticado
    /// </summary>
    public Usuario? Usuario { get; set; }

    /// <summary>
    /// Crea un resultado de login fallido
    /// </summary>
    /// <param name="error"></param>
    /// <returns>Un LoginResult con éxito falso y el mensaje de error proporcionado</returns>
    public static LoginResult Failed(string error) => new() { Success = false, Error = error };

    /// <summary>
    /// Crea un resultado de login exitoso  
    /// </summary>
    /// <param name="usuario"></param>
    /// <returns>Un LoginResult con éxito verdadero y el usuario proporcionado</returns>
    public static LoginResult Ok(Usuario usuario) => new() { Success = true, Usuario = usuario };
}
