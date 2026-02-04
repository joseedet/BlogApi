using System;

namespace BlogApi.DTO;

/// <summary>
/// Dto par actualizar página
/// </summary>
public class ActualizarPageDto
{
    /// <summary>
    /// Título
    /// </summary>
    public string Titulo { get; set; }

    /// <summary>
    /// Contenido
    /// </summary>
    public string Contenido { get; set; }

    /// <summary>
    /// Si está publicado  o no.
    /// </summary>
    public bool Publicado { get; set; }
}
