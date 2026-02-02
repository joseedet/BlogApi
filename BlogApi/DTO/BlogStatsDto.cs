using System;

namespace BlogApi.DTO;

/// <summary>
/// Estadísticas
/// </summary>
public class BlogStatsDto
{
    /// <summary>
    /// Total Posts
    /// </summary>
    public int TotalPosts { get; set; }

    /// <summary>
    /// Total categorías
    /// </summary>
    public int TotalCategorias { get; set; }

    /// <summary>
    /// Total Tags
    /// </summary>
    public int TotalTags { get; set; }

    /// <summary>
    /// Total Usuarios
    /// </summary>
    public int TotalUsuarios { get; set; }

    /// <summary>
    /// Total Comentarios.
    /// </summary>
    public int TotalComentarios { get; set; }
}
