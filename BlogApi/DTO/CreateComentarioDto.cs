using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.DTO;

public class CreateComentarioDto
{
    [Required]
    [MinLength(2)]
    public string Contenido { get; set; } = string.Empty;

    [Required]
    public int PostId { get; set; }

    //public int? UsuarioId { get; set; }
    public int? ComentarioPadreId { get; set; }
}
