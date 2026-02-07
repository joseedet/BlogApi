using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Utils.Enums;

namespace BlogApi.Models;

/// <summary>
/// Clase Post
/// </summary>
public class Post
{
    /// <summary>
    /// Identificador el post
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Enumeración en la cual se muestran los distintos estados de un post.
    /// </summary>
    public PostEstado Estado { get; set; } = PostEstado.Borrador;

    /// <summary>
    /// titulo del post
    /// </summary>
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Contenido del post
    /// </summary>
    public string Contenido { get; set; } = string.Empty;

    /// <summary>
    /// Id del usuario
    /// </summary>
    public int UsuarioId { get; set; }

    /// <summary>
    /// Navegación usuario
    /// </summary>
    public Usuario Usuario { get; set; } = null;

    /// <summary>
    /// Id Categoria
    /// </summary>
    public int CategoriaId { get; set; }

    /// <summary>
    /// Enlace de navegación para categoría
    /// </summary>
    public Categoria Categoria { get; set; } = null;

    /// <summary>
    /// Fecha creación del post
    /// </summary>
    public DateTime FechaCreacion { get; set; }

    /// <summary>
    /// Slug
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Contador de visitas
    /// </summary>
    public int ViewsCount { get; set; }

    /// <summary>
    /// Fecha de actualización
    /// </summary>
    public DateTime FechaActualizacion { get; set; }

    /// <summary>
    /// Indica si el post está publicado o no
     /// </summary>
    /// </summary>
    public bool Publicado { get; set; }

    /// <summary>
    /// Fecha de publicación (si está publicado)
    /// </summary>
    public DateTime? FechaPublicacion { get; set; }

    /// <summary>
    ///     Indica si el post es destacado o no   
    /// </summary>
    public bool Destacado { get; set; }

    /// <summary>
    /// Lista de comentario
    /// </summary>
    public List<Comentario> Comentarios { get; set; } = new();

    /// <summary>
    /// Lista de tags
    /// </summary>
    public List<Tag> Tags { get; set; } = new();
}
