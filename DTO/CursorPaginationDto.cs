using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.DTO;

public class CursorPaginationDto<T>
{
    public IEnumerable<T> Datos { get; set; } = new List<T>();
    public int? NextCursor { get; set; }
}
