using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.Models;

/// <summary>
///     Modelo que representa un "like" en una publicación
/// </summary>
public class LikePost
{
    /// <summary>
    ///     Identificador único del "like"
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///    Identificador de la publicación a la que se le dio "like"
    /// </summary>
    public int PostId { get; set; }

    /// <summary>
    ///   Publicación a la que se le dio "like"
    /// </summary>
    public Post Post { get; set; } = null!;

    /// <summary>
    ///   Identificador del usuario que dio el "like"
    /// </summary>
    public int UsuarioId { get; set; }

    /// <summary>
    ///   Usuario que dio el "like"
    /// </summary>
    public Usuario Usuario { get; set; } = null!;
}
