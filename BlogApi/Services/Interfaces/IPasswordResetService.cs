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
}
