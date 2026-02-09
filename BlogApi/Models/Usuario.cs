using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    /// Apellidos del usuario.
    /// </summary>
    public string Apellidos { get; set; } = string.Empty;

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

    /// <summary>
    /// Url de la imagen
    /// </summary>

    public string AvatarUrl { get; set; } = string.Empty;

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

    // Nuevo sistema de verificación de email

    /// <summary> Indica si el correo electrónico ha sido verificado. </summary>
    public bool EmailVerificado { get; set; }
    /// <summary>
    /// Email verificado en
    /// </summary>
    public DateTime? EmailVerificadoEn { get; set; }

    /// <summary>
    /// Salt para la verificación de email
    /// </summary>
    public string? EmailVerificationSalt { get; set; }

    /// <summary>
    /// Indica si el usuario está bloqueado debido a intentos fallidos de inicio de sesión o por otras razones administrativas.
    /// </summary>
    public bool EstaBloqueado { get; set; }


    /// <summary>
    /// Lista de tokens refrescados
    /// </summary>
    public List<RefreshToken> RefreshTokens { get; set; } = new();
    
    /// <summary>
    /// Usuario Roles
    /// </summary>
    public List<UsuarioRol> UsuarioRoles { get; set; } = new();
}
