using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.Models;

public class Comentario
{
    public int Id { get; set; }
    public string Contenido { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    // Relación con Post
    public int PostId { get; set; }
    public Post Post { get; set; }

    // Relación con Usuario (opcional)
    public int? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    // Moderación
    public string Estado { get; set; } = "pendiente";

    // Comentarios anidados (como WordPress)
    public int? ComentarioPadreId { get; set; }
    public Comentario? ComentarioPadre { get; set; }
    public List<Comentario> Respuestas { get; set; } = new();
}
