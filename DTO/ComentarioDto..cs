using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.DTO;

public class ComentarioDto
{
    public int Id { get; set; }
    public string Contenido { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = "pendiente";
    public UsuarioDto? Usuario { get; set; }
    public List<ComentarioDto> Respuestas { get; set; } = new();
}
