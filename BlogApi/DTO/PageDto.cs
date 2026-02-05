using System;

namespace BlogApi.DTO;

/// <summary>
/// Dto Page
/// </summary>
public class PageDto
{
    /// <summary>
    /// Identificador
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Título
    /// </summary>
    public string Titulo { get; set; }

    /// <summary>
    /// Slug
    /// </summary>
    public string Slug { get; set; }

    /// <summary>
    /// Contenido
    /// </summary>
    public string Contenido { get; set; }

    /// <summary>
    /// Publicado
    /// </summary>
    public bool Publicado { get; set; }

    /// <summary>
    /// Fecha de creación
    /// </summary>
    public DateTime Creado { get; set; }

    /// <summary>
    /// Fecha de actualización
    /// </summary>
    public DateTime Actualizado { get; set; }

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

    /// <summary>
    /// IPCreación
    /// </summary>
    public string IpCreacion { get; set; }

    /// <summary>
    /// UserAgentCreacion
    /// </summary>
    public string UserAgentCreacion { get; set; }

    /// <summary>
    /// IpActualizacion
    /// </summary>
    public string IpActualizacion { get; set; }

    /// <summary>
    /// /// UserAgentActualizacion
    /// </summary>
    public string UserAgentActualizacion { get; set; }
}
