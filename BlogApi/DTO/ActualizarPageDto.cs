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

    /// <summary>
    /// Si es una página de inicio
    /// </summary>
    public bool EsInicio { get; set; }

    /// <summary>
    /// MetaTitulo
    /// </summary>
    public string MetaTitulo { get; set; }

    /// <summary>
    /// MetaDescripcion
    /// </summary>
    public string MetaDescripcion { get; set; }

    /// <summary>
    /// MetaKeywords
    /// </summary>
    public string MetaKeywords { get; set; }
}
