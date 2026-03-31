using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Data;
using Microsoft.EntityFrameworkCore;
using BlogApi.Services.Interfaces;
using BlogApi.Models;

namespace BlogApi.Services;
/// <summary>
/// Servicio para manejar la lógica relacionada con el menú de la aplicación. Este servicio se encarga de obtener la estructura del menú desde la base de datos, organizando los elementos en una jerarquía si es necesario. El método GetMenuAsync devuelve una lista de objetos MenuItem que representan los elementos del menú, incluyendo sus hijos si existen. El servicio utiliza Entity Framework Core para acceder a los datos de manera asíncrona y eficiente.
/// </summary>
public class MenuService: IMenuService
{
    private readonly BlogDbContext _context;

    public MenuService(BlogDbContext context)
    {
        _context = context;
    }
     public async Task<List<MenuItem>> GetMenuAsync()
    {
        var items = await _context.MenuItems
            .Where(x => x.IsActive)
            .OrderBy(x => x.Order)
            .ToListAsync();

        var lookup = items.ToDictionary(x => x.Id);
        var rootItems = new List<MenuItem>();

        foreach (var item in items)
        {
            if (item.ParentId == null)
            {
                rootItems.Add(item);
            }
            else
            {
                if (lookup.TryGetValue(item.ParentId.Value, out var parent))
{                    parent.Children.Add(item);
                }
            }
        }

        return rootItems;
    }
}
