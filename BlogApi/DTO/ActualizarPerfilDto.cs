using System;

namespace BlogApi.DTO;

/// <summary>
/// Actualiza el perfil mediante Dto.
/// </summary>
public class ActualizarPerfilDto
{
    /// <summary>
    /// Nombre
    /// </summary>
    public string Nombre { get; set; }

    /// <summary>
    /// Apellidos
    /// </summary>
    public string Apellidos { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; }
}
