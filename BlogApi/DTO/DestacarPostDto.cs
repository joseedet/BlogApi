using System;

namespace BlogApi.DTO;

/// <summary> 
/// DTO para marcar o desmarcar un post como destacado.
/// </summary>
public class DestacarPostDto
{
    /// <summary>
    /// Indica si el post debe destacarse (true) o quitarse de destacados (false).
    /// </summary>
    public bool Destacar { get; set; }
}
