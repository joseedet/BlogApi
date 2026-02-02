using System;
using BlogApi.Models;

namespace BlogApi.DTO;


/// <summary>
/// Actividad reciente Dto.
/// </summary>
public class ActividadRecienteDto
{
    /// <summary>
    /// Últimos posts.
    /// </summary>
    public IEnumerable<Post> UltimosPosts { get; set; }

    /// <summary>
    /// Ultimos Comentarios.
    /// </summary>
    public IEnumerable<Comentario> UltimosComentarios { get; set; }

    /// <summary>
    /// Ultimos usuarios
    /// </summary>


    public IEnumerable<Usuario> UltimosUsuarios { get; set; }
}
