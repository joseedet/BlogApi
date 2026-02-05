using System;

namespace BlogApi.Models;

/// <summary>
/// Página
/// </summary>
public class Page
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
    /// Fecha de Creación
    /// </summary>
    public DateTime Creado { get; set; }

    /// <summary>
    /// Fecha de actualización
    /// </summary>
    public DateTime Actualizado { get; set; }

    // Auditoría (opcional, pero coherente con lo que ya haces)

    /// <summary>
    /// IpCreacion
    /// </summary>
    public string IpCreacion { get; set; }

    /// <summary>
    /// Cración User-Agent
    /// </summary>
    public string UserAgentCreacion { get; set; }

    /// <summary>
    /// Ip de Actualización
    /// </summary>
    public string IpActualizacion { get; set; }

    /// <summary>
    /// User-Agent de actualizacón
    /// </summary>
    public string UserAgentActualizacion { get; set; }

    /// <summary>
    /// Publicado
    /// </summary>
    // Opcional: para ocultar sin borrar
    public bool Publicado { get; set; }

    /// <summary>
    /// Si es una página de inicio
    /// </summary>
    public bool EsInicio { get; set; }

    /// <summary>
    /// MetaTítulo
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
