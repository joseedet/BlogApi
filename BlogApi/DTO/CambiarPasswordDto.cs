using System;

namespace BlogApi.DTO;

/// <summary>
/// Cambiar password Dto.
/// </summary>
public class CambiarPasswordDto
{
    /// <summary>
    /// Password Actual
    /// </summary>
    public string PasswordActual { get; set; }

    /// <summary>
    /// Nuevo password
    /// </summary>
    public string NuevaPassword { get; set; }
}
