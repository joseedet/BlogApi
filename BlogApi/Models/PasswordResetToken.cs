using System;

namespace BlogApi.Models;

/// <summary>
/// Token para resetear la contraseña
/// </summary>
public class PasswordResetToken
{
    /// <summary>
    /// Identificador
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Token Hash
    /// </summary>
    // Guardamos el hash del token, no el token en claro
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de expiración
    /// </summary>
    public DateTime Expira { get; set; }

    /// <summary>
    /// Fecha de creación
    /// </summary>
    public DateTime Creado { get; set; }

    /// <summary>
    /// Fecha en la cual ha sido usado
    /// </summary>
    public DateTime? Usado { get; set; }

    /// <summary>
    /// Identificación del usuario
    /// </summary>
    // Relación con Usuario
    public int UsuarioId { get; set; }

    /// <summary>
    /// LLave de navegación
    /// </summary>
    public Usuario Usuario { get; set; } = null!;

    /// <summary>
    /// Devuelve verdadero si esta activo en caso contrario falso
    /// </summary>
    public bool EstaActivo => Usado == null && !EstaExpirado;

    /// <summary>
    /// Devuelve verdadero si está expirado en caso contrario falso
    /// </summary>
    public bool EstaExpirado => DateTime.UtcNow >= Expira;
}
