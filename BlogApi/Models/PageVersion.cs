using System;

namespace BlogApi.Models;

/// <summary>
/// Versión de la página
/// </summary>
public class PageVersion
{
    /// <summary>
    /// Identificador
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador de la página
    /// </summary>
    public int PageId { get; set; }

    /// <summary>
    /// Enlace de navegación
    /// </summary>
    public Page Page { get; set; }

    /// <summary>
    /// Título de la página
    /// </summary>
    public string Titulo { get; set; }

    /// <summary>
    /// Slug de la página
    /// </summary>
    public string Slug { get; set; }

    /// <summary>
    /// Contenido
    /// </summary>
    public string Contenido { get; set; }

    /// <summary>
    /// Si se ha publicado
    /// </summary>
    public bool Publicado { get; set; }

    /// <summary>
    /// Si es página de inicio
    /// </summary>
    public bool EsInicio { get; set; }

    /// <summary>
    /// Fecha de la versión
    /// </summary>
    public DateTime FechaVersion { get; set; }

    /// <summary>
    /// Ip Creación
    /// </summary>
    public string IpCreacion { get; set; }

    /// <summary>
    /// User-Agent creación
    /// </summary>
    public string UserAgentCreacion { get; set; }
}
