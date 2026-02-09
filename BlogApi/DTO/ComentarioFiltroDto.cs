using System;

namespace BlogApi.DTO;

/// <summary>
/// DTO para filtrar comentarios. Permite filtrar por ID de post, ID de usuario y estado del comentario, así como paginar los resultados.
/// </summary>
public class ComentarioFiltroDto
{
    /// <summary>
    /// ID del post al que pertenecen los comentarios a filtrar. Es un campo opcional, ya que se pueden filtrar comentarios de todos los posts.
    /// </summary>
    public int? PostId { get; set; }

    /// <summary>
    ///     ID del usuario autor de los comentarios a filtrar. Es un campo opcional, ya que se pueden filtrar comentarios de todos los usuarios.
    /// </summary>
    public int? UsuarioId { get; set; }

    /// <summary>
    /// Estado de los comentarios a filtrar. Es un campo opcional, ya que se pueden filtrar comentarios de todos los estados. Los posibles valores son "Pendiente", "Aprobado" y "Rechazado".
    /// </summary>
    public string? Estado { get; set; }

    // "Pendiente", "Aprobado", "Rechazado"
    /// <summary>
    /// Número de página para paginar los resultados. Es un campo opcional, con un valor predeterminado de 1.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Número de comentarios por página para paginar los resultados. Es un campo opcional, con un valor predeterminado de 20.
    /// </summary>
    public int PageSize { get; set; } = 20;
}
