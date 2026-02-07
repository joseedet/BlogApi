using System;

namespace BlogApi.DTO;

/// <summary>
/// DTO para publicar o despublicar un post.
/// </summary>
public class PublicarPostDto
{
    /// <summary>
    /// Indica si el post debe publicarse (true) o despublicarse (false).
    /// </summary>
    public bool Publicar { get; set; }
}
