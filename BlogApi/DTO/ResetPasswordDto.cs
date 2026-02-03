using System;

namespace BlogApi.DTO;

/// <summary>
/// Resetear password Dto.
/// </summary>
public class ResetPasswordDto
{
    /// <summary>
    /// Correo electrónico
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Token
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Nueva contraseña
    /// </summary>
    public string NuevaPassword { get; set; }
}
