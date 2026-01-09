using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.DTO;

/// <summary>
///   Data Transfer Object para los "Me gusta"
/// </summary>
public class LikeDto
{
    /// <summary>
    ///   Id del usuario que da el "Me gusta"
    /// </summary>
    public int UsuarioId { get; set; }

    /// <summary>
    ///  Indica si al usuario le gusta el contenido
    /// </summary>
    public bool LeGusta { get; set; }
}
