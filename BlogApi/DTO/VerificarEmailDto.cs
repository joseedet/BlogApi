using System;

namespace BlogApi.DTO;

/// <summary>
/// DTO para verificar el email del usuario
/// </summary>
public class VerificarEmailDto
{
    /// <summary>
    /// Token de verificación
    /// </summary>
    public string Token { get; set; } = string.Empty;
}
