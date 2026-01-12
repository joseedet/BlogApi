using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.DTO;

/// <summary>
/// DTO para paginación con cursor
/// </summary>
/// <typeparam name="T"></typeparam>
public class CursorPaginationDto<T>
{
    /// <summary>
    /// Elementos de la página actual
    /// </summary>
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    /// <summary>
    /// Cursor para la siguiente página
    /// </summary>
    public int? NextCursor { get; set; }

    /// <summary>
    /// Constructor de CursorPaginationDto
    /// </summary>
    public CursorPaginationDto(IEnumerable<T> items, int? nextCursor)
    {
        Items = items;
        NextCursor = nextCursor;
    }

    /// <summary>
    /// Constructor vacío de CursorPaginationDto
    /// </summary>
    public CursorPaginationDto() { }
}
