using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.DTO;

/// <summary>
///  Data Transfer Object para los "Me gusta" de un post
/// </summary>
public class PostLikesDto
{
    /// <summary>
    ///  Id del post
    /// </summary>
    public int PostId { get; set; }

    /// <summary>
    ///  Total de "Me gusta"
    /// </summary>
    public int TotalLikes { get; set; }

    /// <summary>
    ///  Indica si el usuario ha dado "Me gusta" al post
    /// </summary>
    public bool UsuarioHaDadoLike { get; set; }
}
