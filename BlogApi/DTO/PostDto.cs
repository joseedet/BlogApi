using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.DTO;

/// <summary>
/// Transferencia de objeto para Post
/// </summary>
public class PostDto
{
    /// <summary>
    /// ID del post
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Titulo del post
    /// </summary>
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Contenido del post
    /// </summary>
    public string Contenido { get; set; } = string.Empty;

    /// <summary>
    /// Slug para la URL del post
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de creación del post
    /// </summary>
    public DateTime FechaCreacion { get; set; }

    /// <summary>
    /// Fecha de actualización del post
    /// </summary>
    public DateTime FechaActualizacion { get; set; }

    /// <summary>
    /// Categoría del post
    /// </summary>
    public CategoriaDto Categoria { get; set; }

    /// <summary>
    /// Usuario que creó el post
    /// </summary>
    public UsuarioDto Usuario { get; set; }

    /// <summary>
    /// Comentarios del post
    /// </summary>
    public List<ComentarioDto> Comentarios { get; set; } = new();

    /// <summary>
    /// Etiquetas del post
    /// </summary>
    public List<TagDto> Tags { get; set; } = new();
}
