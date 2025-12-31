using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.Models;

public class Tag
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public List<Post> Posts { get; set; } = new();
}
