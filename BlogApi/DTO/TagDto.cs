using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.DTO;

public class TagDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
