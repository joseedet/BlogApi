using System;

namespace BlogApi.DTO;

/// <summary>
/// Dto para crear pagina
/// </summary>
public class CrearPageDto
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
    /// Si está publicado
    /// </summary>
    public bool Publicado { get; set; }

    /// <summary>
    /// Si es una página de inicio
    /// </summary>
    public bool EsInicio { get; set; }

}
