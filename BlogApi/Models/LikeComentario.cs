using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.Models;

/// <summary>
///     Modelo que representa un "like" en un comentario
/// </summary>
public class LikeComentario
{
    /// <summary>
    ///     Identificador único del "like"
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Identificador del comentario al que se le dio "like"
    /// </summary>
    public int ComentarioId { get; set; }

    /// <summary>
    ///     Comentario al que se le dio "like"
    /// </summary>
    public Comentario Comentario { get; set; } = null!;

    /// <summary>
    ///     Identificador del usuario que dio el "like"
    /// </summary>
    public int UsuarioId { get; set; }

    /// <summary>
    ///     Usuario que dio el "like"
    /// </summary>
    public Usuario Usuario { get; set; } = null!;
}
