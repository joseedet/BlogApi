using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Models;

namespace BlogApi.DTO;

public class PaginationDto<T>
{
    public int Pagina { get; set; }
    public int Tamano { get; set; }
    public int Total { get; set; }
    public IEnumerable<T> Datos { get; set; } = new List<T>();
}
