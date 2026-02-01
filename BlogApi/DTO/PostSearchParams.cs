using System;
using BlogApi.Utils.Enums;

namespace BlogApi.DTO;

/// <summary>
/// Clase busqueda con varios parámetros
/// </summary>
public class PostSearchParams
{
    /// <summary>
    /// Texto
    /// </summary>
    public string? Texto { get; set; }

    /// <summary>
    /// /Id de la categoría
    /// </summary>
    public int? CategoriaId { get; set; }

    /// <summary>
    /// Categoria / Slug
    /// </summary>
    public string? CategoriaSlug { get; set; }

    /// <summary>
    /// Id del autor
    /// </summary>
    public int? AutorId { get; set; }

    /// <summary>
    /// Nombre del autor
    /// </summary>
    public string? AutorNombre { get; set; }

    /// <summary>
    /// Fecha de inicio de búsqueda
    /// </summary>
    public DateTime? Desde { get; set; }

    /// <summary>
    /// Fecha de fin de búsqueda
    /// </summary>
    public DateTime? Hasta { get; set; }

    /// <summary>
    /// Estado en el cual encuentra el post
    /// </summary>
    public PostEstado? Estado { get; set; }
}
