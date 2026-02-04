using System;

namespace BlogApi.DTO;

/// <summary>
/// Dto para el listdo de paginas
/// </summary>
public class PageListadoDto
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
    /// Si la página ha sido publicada o no
    /// </summary>
    public bool Publicado { get; set; }

    /// <summary>
    /// Fecha de la actualización
    /// </summary>
    public DateTime Actualizado { get; set; }
}
