using System;
using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

/// <summary>
/// Servicio para manejar operaciones relacionadas con los permisos de usuario en el sistema, como listar permisos disponibles (solo para admin/panel)
/// </summary>
public class PermisoService : IPermisoService
{
    private readonly BlogDbContext _context;
    /// <summary>
    /// Constructor que recibe el contexto de la base de datos a través de inyección de dependencias
    /// </summary>
    /// <param name="context"></param>
    public PermisoService(BlogDbContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Obtiene todos los permisos disponibles en la base de datos, ordenados por nombre
    /// </summary>
    /// <returns>List&lt;Permiso&gt;</returns>
    public async Task<List<Permiso>> GetAllAsync()
    {
        return await _context.Permisos.OrderBy(p => p.Clave).AsAsyncEnumerable().ToListAsync();
    }
}
