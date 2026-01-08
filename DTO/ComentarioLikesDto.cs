using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.DTO;

/// <summary>
/// Data Transfer Object para los "Me gusta" de un comentario
/// </summary>
public class ComentarioLikesDto
{
    /// <summary>
    /// Id del comentario
    /// </summary>
    public int ComentarioId { get; set; }

    /// <summary>
    /// Total de "Me gusta"
    /// </summary>
    public int TotalLikes { get; set; }

    /// <summary>
    /// Indica si el usuario ha dado "Me gusta" al comentario
    /// </summary>
    public bool UsuarioHaDadoLike { get; set; }

    /// <summary>
    /// Id del usuario que dio el like
    /// </summary>
    public int UsuarioId { get; set; }
}
