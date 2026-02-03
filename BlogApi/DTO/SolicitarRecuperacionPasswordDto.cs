using System;

namespace BlogApi.DTO;

/// <summary>
/// Solicita la recuperacion del password Dto.
/// </summary>
public class SolicitarRecuperacionPasswordDto
{
    /// <summary>
    /// Email para la solicitud de recuperación de la contraseña
    /// </summary>
    public string Email { get; set; } = string.Empty;
}
