using System.ComponentModel.DataAnnotations;

namespace BlogApi.DTO;

/// <summary>
/// DTO para crear un comentario. Contiene el contenido del comentario, el ID del post al que pertenece, y opcionalmente el ID del comentario padre para respuestas anidadas.
/// </summary>
public class CreateComentarioDto
{
    /// <summary>
    /// Contenido del comentario. Es un campo obligatorio con una longitud mínima de 2 caracteres.
    /// </summary>
    [Required]
    [MinLength(2)]
    public string Contenido { get; set; } = string.Empty;

    /// <summary>
    ///     ID del post al que pertenece el comentario. Es un campo obligatorio.
    /// </summary>
    [Required]
    public int PostId { get; set; }

    //public int? UsuarioId { get; set; }
    /// <summary>
    /// ID del comentario padre para respuestas anidadas. Es un campo opcional, ya que no todos los comentarios serán respuestas a otros comentarios.
    /// </summary>
    public int? ComentarioPadreId { get; set; }
}
