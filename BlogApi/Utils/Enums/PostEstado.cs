namespace BlogApi.Utils.Enums;

/// <summary>
/// Enumeración que indicará el tipo de estado en el que se encuentra el post.
/// </summary>
public enum PostEstado
{
    /// <summary>
    /// Estado borrador
    /// </summary>
    Borrador = 0,

    /// <summary>
    /// Estado publicado
    /// </summary>
    Publicado = 1,

    /// <summary>
    /// Archivado
    /// </summary>
    Archivado = 2,
}
