using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Utils.Enums;

namespace BlogApi.Models;

/// <summary>
/// Clase comentario
/// </summary>
public class Comentario
{
    /// <summary>
    /// Identificador
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Contenido
    /// </summary>
    public string Contenido { get; set; } = string.Empty;

    //public DateTime Fecha { get; set; } = DateTime.UtcNow;

    // Relación con Post
    /// <summary>
    /// Identificador del post
    /// </summary>
    public int PostId { get; set; }

    /// <summary>
    /// Propiedad de navegación
    /// </summary>
    public Post Post { get; set; }

    // Relación con Usuario (opcional)
    /// <summary>
    /// Identificador del usuario
    /// </summary>
    public int? UsuarioId { get; set; }

    /// <summary>
    /// Propiedad de navegación del usuario
    /// </summary>
    public Usuario? Usuario { get; set; }

    // Moderación
    /// <summary>
    /// Estado en el que se encuentra el comentario
    /// </summary>
    public ComentarioEstado Estado { get; set; } = ComentarioEstado.Pendiente;

    // Comentarios anidados (como WordPress)
    /// <summary>
    /// Identificador del comentario padre
    /// </summary>
    public int? ComentarioPadreId { get; set; }

    /// <summary>
    /// Comentario padre
    /// </summary>
    public Comentario? ComentarioPadre { get; set; }

    /// <summary>
    /// Lista de respuestas del comentario.
    /// </summary>
    public List<Comentario> Respuestas { get; set; } = new();

    /// <summary>
    /// Fecha de creación del comentario.
    /// </summary>
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
