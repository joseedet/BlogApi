using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.Models;

/// <summary>
/// Tag
/// </summary>
public class Tag
{
    /// <summary>
    /// Identificador
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del tag
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Slug asociado al tag
    /// /// </summary>
    public string Slug { get; set; } = null!;

    /// <summary>
    /// Lista de Post
    /// </summary>
    public List<Post> Posts { get; set; } = new();
}
