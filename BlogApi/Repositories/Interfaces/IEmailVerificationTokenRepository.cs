using System;
using BlogApi.Models;

namespace BlogApi.Repositories.Interfaces;

/// <summary>
/// Interfaz para la verificación del token
/// </summary>
public interface IEmailVerificationTokenRepository
{
    /// <summary> ///
    ///  Crea un nuevo token de verificación (hash + expiración + auditoría). 
    /// </summary> 
    Task CrearAsync(EmailVerificationToken token);

    /// <summary> 
    ///  Obtiene el token activo del usuario:
    /// - No usado
    /// - No expirado 
    /// </summary>
    Task<EmailVerificationToken?> ObtenerTokenActivoAsync(int userId);

    /// <summary> 
    /// Obtiene todos los reenvíos realizados en la última hora
    /// para aplicar la regla de máximo 3 reenvíos/hora. 
    /// </summary>
    Task<IEnumerable<EmailVerificationToken>> ObtenerReenviosUltimaHoraAsync(int userId);

    /// <summary>  
    /// Busca un token por su hash (SHA-512(token + salt)). 
    /// </summary> 
    Task<EmailVerificationToken?> ObtenerPorHashAsync(string tokenHash);

    /// <summary>
    /// Actualiza un token existente:
    /// - Marcar como usado
    /// - Incrementar reenvíos
    /// - Registrar auditoría
    /// </summary>
    Task ActualizarAsync(EmailVerificationToken token);
}
