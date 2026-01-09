using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.DTO;

public class PostDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
    public CategoriaDto Categoria { get; set; }
    public UsuarioDto Usuario { get; set; }
    public List<ComentarioDto> Comentarios { get; set; } = new();
    public List<TagDto> Tags { get; set; } = new();
}
