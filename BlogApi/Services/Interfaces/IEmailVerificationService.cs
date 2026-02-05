using BlogApi.Models;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Interfaz Email Verification Service
/// </summary>
public interface IEmailVerificationService
{
    /// <summary>
    /// Geneera el envio de Token
    /// </summary>
    /// <param name="usuario"></param>
    /// <param name="ip"></param>
    /// <param name="userAgent"></param>
    /// <returns></returns>
    Task GenerarYEnviarTokenAsync(Usuario usuario, string ip, string userAgent);

    /// <summary>
    /// Verifica el Token
    /// </summary>
    /// <param name="tokenPlano"></param>
    /// <param name="ip"></param>
    /// <param name="userAgent"></param>
    /// <returns>Devuelve verdadero si se ha verificado el token en caso contrario falso</returns>
    Task<bool> VerificarTokenAsync(string tokenPlano, string ip, string userAgent);

    /// <summary>
    /// Reenvia el Token si es necesario
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ip"></param>
    /// <param name="userAgent"></param>
    /// <returns>Devuelve verdadero si es preciso reenviar el token en caso contrario falso</returns>
    Task<bool> ReenviarTokenAsync(int userId, string ip, string userAgent);
}
