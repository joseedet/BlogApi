using System;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Interfaz para implementar el servicio de recuperación de contraseña.
/// </summary>
public interface IPasswordResetService
{
    /// <summary>
    /// Método para solicitar el correo de recuperación de la contraseña.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    Task SolicitarRecuperacionAsync(string email);

    /// <summary>
    /// Valida el Token
    /// </summary>
    /// <param name="email"></param>
    /// <param name="tokenPlano"></param>
    /// <returns>Devuelve verdadero si está ok en caso contrario falso</returns>
    Task<bool> ValidarTokenAsync(string email, string tokenPlano);

    /// <summary>
    /// Resetear contraseña
    /// </summary>
    /// <param name="email"></param>
    /// <param name="tokenPlano"></param>
    /// <param name="nuevaPassword"></param>
    /// <returns>Devuelve verdadero si se ha podido en caso contrario falso</returns>
    Task<bool> ResetPasswordAsync(string email, string tokenPlano, string nuevaPassword);
}
