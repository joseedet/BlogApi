using System;

namespace BlogApi.DTO;

/// <summary>
/// DTO para filtrar usuarios. Permite filtrar por rol, estado de bloqueo y búsqueda por nombre o email, así como paginar los resultados.
/// </summary>
public class UsuarioFiltroDto
{
    /// <summary>
    /// Rol del usuario a filtrar. Es un campo opcional, ya que se pueden filtrar usuarios de todos los roles. Los posibles valores son "Admin", "Autor" y "Lector".
    ///
    /// </summary>
    public string? Rol { get; set; }

    /// <summary>
    ///     Indica si se deben filtrar usuarios bloqueados o no bloqueados. Es un campo opcional, ya que se pueden filtrar usuarios de ambos estados. Si es true, se filtrarán solo los usuarios bloqueados; si es false, se filtrarán solo los usuarios no bloqueados; si es null, se filtrarán usuarios de ambos estados.
    /// </summary>
    public bool? Bloqueado { get; set; }

    /// <summary>
    /// Término de búsqueda para filtrar usuarios por nombre o email. Es un campo opcional, ya que se pueden filtrar usuarios sin un término de búsqueda específico. Si se proporciona, se filtrarán los usuarios cuyo nombre o email contenga el término de búsqueda (sin distinguir mayúsculas).
    /// </summary>
    /// <remarks>
    /// El término de búsqueda se aplicará tanto al nombre como al email del usuario. Por ejemplo, si el término de búsqueda es "juan", se filtrarán los usuarios cuyo nombre o email contenga "juan", como "Juan Pérez" o "
    /// </remarks>
    public string? Buscar { get; set; } // nombre o email

    /// <summary>
    /// Número de página para paginar los resultados. Es un campo opcional, con un valor predeterminado de 1.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Número de usuarios por página para paginar los resultados. Es un campo opcional, con un valor predeterminado de 20.
    /// </summary>
    public int PageSize { get; set; } = 20;
}
