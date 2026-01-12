namespace BlogApi.DTO;

/// <summary>
/// DTO para crear un nuevo post
/// </summary>
public class CreatePostDto
{
    /// <summary>
    /// Título del post
    /// </summary>
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Contenido del post
    /// </summary>
    public string Contenido { get; set; } = string.Empty;

    /// <summary>
    /// ID de la categoría del post
    /// </summary>
    public int CategoriaId { get; set; }

    /// <summary>
    /// ID del usuario que crea el post
    /// </summary>
    public int UsuarioId { get; set; }

    /// <summary>
    /// IDs de los tags asociados al post
    /// </summary>
    public List<int> TagIds { get; set; } = new();
}
