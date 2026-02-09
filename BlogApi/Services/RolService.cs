using System;
using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace BlogApi.Services;

/// <summary>
/// Servicio para manejar operaciones relacionadas con roles de usuario
/// </summary>
public class RolService : IRolService
{
    /// <summary>
    /// Contexto de la base de datos para acceder a los roles
    /// </summary>
    private readonly BlogDbContext _context;

    /// <summary>
    /// Constructor que recibe el contexto de la base de datos a través de inyección de dependencias
    /// </summary>
    /// <param name="context"></param>

    public RolService(BlogDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todos los roles disponibles en la base de datos, ordenados por nombre
    /// </summary>
    /// <returns>List&lt;Rol&gt;</returns>/

    public async Task<List<Rol>> GetAllAsync()
    {
        return await _context.Roles.OrderBy(r => r.Nombre).ToListAsync();
    }
}
