using System;
using BlogApi.Data;
using BlogApi.DTO;
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

    /// <summary>
    /// Crea un nuevo permiso en la base de datos utilizando los datos proporcionados en el DTO CrearPermisoDto, devuelve el permiso creado con su ID asignado, este método se utiliza para agregar nuevos permisos al sistema desde la interfaz de administración o panel de control (solo accesible para usuarios con permisos de administración)
    /// </summary>
    public async Task<Permiso?> CrearPermisoAsync(CrearPermisoDto dto)
    {
        // Validar que no exista un permiso con la misma clave
        var existe = await _context.Permisos.AnyAsync(p =>
            p.Clave.ToLower() == dto.Clave.ToLower()
        );

        if (existe)
            return null;

        var permiso = new Permiso
        {
            Clave = dto.Clave.Trim(),
            Descripcion = dto.Descripcion.Trim(),
        };

        _context.Permisos.Add(permiso);
        await _context.SaveChangesAsync();

        return permiso;
    }

    /// <summary>
    /// Edita un permiso existente en la base de datos utilizando los datos proporcionados en el DTO EditarPermisoDto, devuelve un booleano indicando si la operación fue exitosa o no, este método se utiliza para actualizar los permisos existentes en el sistema desde la interfaz de administración o panel de control (solo accesible para usuarios con permisos de administración)
    /// </summary>
    /// <param name="permisoId"></param>
    /// <param name="dto"></param>
    /// <returns>Booleano indicando si la operación fue exitosa o no</returns>
    public async Task<bool> EditarPermisoAsync(int permisoId, EditarPermisoDto dto)
    {
        var permiso = await _context.Permisos.FirstOrDefaultAsync(p => p.Id == permisoId);

        if (permiso == null)
            return false;

        // Validar que no exista otro permiso con la misma clave
        var claveExiste = await _context.Permisos.AnyAsync(p =>
            p.Id != permisoId && p.Clave.ToLower() == dto.Clave.ToLower()
        );

        if (claveExiste)
            return false;

        permiso.Clave = dto.Clave.Trim();
        permiso.Descripcion = dto.Descripcion.Trim();

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Elimina un permiso existente en la base de datos utilizando el ID del permiso a eliminar, devuelve un booleano indicando si la operación fue exitosa o no, este método se utiliza para eliminar permisos existentes en el sistema desde la interfaz de administración o panel de control (solo accesible para usuarios con permisos de administración)
    /// </summary>
    /// <param name="permisoId"></param>
    /// <returns>Booleano indicando si la operación fue exitosa o no</returns>
    public async Task<bool> EliminarPermisoAsync(int permisoId)
    {
        var permiso = await _context
            .Permisos.Include(p => p.RolPermisos)
            .FirstOrDefaultAsync(p => p.Id == permisoId);

        if (permiso == null)
            return false;

        // Eliminar relaciones con roles
        _context.RolPermisos.RemoveRange(permiso.RolPermisos);

        // Eliminar el permiso
        _context.Permisos.Remove(permiso);

        await _context.SaveChangesAsync();
        return true;
    }
}
