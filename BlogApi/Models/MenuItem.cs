using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.Models;

/// <summary>
/// Representa un  element del menú, que puede ser usado para construir una estructura de navegación jerárquica. Cada elemento del menu puede ser un título, URL, ícono una lista de elem hijos (submenus). O campo ParentId y usarlo para estabelecer una relación entre elementos del menu padre e hijo.
/// </summary>
public class MenuItem
{
    public int Id { get; set; }
    public int? ParentId { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public string Icon { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; }

    public List<MenuItem> Children { get; set; } = new();
}
