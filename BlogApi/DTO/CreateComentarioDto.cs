using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.DTO;

public class CreateComentarioDto
{
    public string Contenido { get; set; } = string.Empty;
    public int PostId { get; set; }
    public int? UsuarioId { get; set; }
    public int? ComentarioPadreId { get; set; }
}
