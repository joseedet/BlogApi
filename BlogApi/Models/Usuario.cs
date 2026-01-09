using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.Models;

/// <summary>
/// Representa un usuario del sistema.
/// </summary>
public class Usuario
{
    /// <summary>
    /// Identificador único del usuario.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del usuario.
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico del usuario.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hash de la contraseña del usuario.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de registro del usuario.
    /// </summary>
    public DateTime FechaRegistro { get; set; }

    // Recuperación de contraseña
    /// <summary>
    /// Token para la recuperación de contraseña.
    /// </summary>
    public string? ResetToken { get; set; }

    /// <summary>
    /// Fecha de expiración del token de recuperación de contraseña.
    /// </summary>
    public DateTime? ResetTokenExpira { get; set; }

    // Protección fuerza bruta

    /// <summary>
    /// Número de intentos fallidos de inicio de sesión.
    /// </summary>
    public int IntentosFallidos { get; set; }

    /// <summary>
    /// Fecha hasta la cual el usuario está bloqueado.
    /// </summary>
    public DateTime? BloqueadoHasta { get; set; }

    /// <summary>
    /// Token para la verificación del correo electrónico.
    /// </summary>
    public string? VerificacionToken { get; set; }

    /// <summary> Fecha de expiración del token de verificación de correo electrónico. </summary>
    public DateTime? VerificacionTokenExpira { get; set; }

    /// <summary> Indica si el correo electrónico ha sido verificado. </summary>
    public bool EmailVerificado { get; set; }

    /// <summary>
    /// Rol del usuario en el sistema.
    /// </summary>
    //  // Protección fuerza bruta public int IntentosFallidos { get; set; } public DateTime? BloqueadoHasta { get; set; }
    public RolUsuario Rol { get; set; } = RolUsuario.Suscriptor;
}
