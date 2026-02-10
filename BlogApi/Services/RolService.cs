using System;
using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;
/// <summary>
/// Servicio para gestionar los roles y sus permisos, permite asignar y quitar permisos a los roles, así como actualizar la lista completa de permisos de un rol
/// </summary>
public class RolService : IRolService
{
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
    /// Actualiza los permisos de un rol específico, eliminando los permisos actuales y asignando la nueva lista de permisos proporcionada
    /// </summary>
    /// <param name="rolId"></param>
    /// <param name="permisosIds"></param>
    /// <returns>True si se actualizó correctamente, false en caso contrario</returns>
    public async Task<bool> ActualizarPermisosRolAsync(int rolId, List<int> permisosIds)
    {
        var rol = await _context
            .Roles.Include(r => r.RolPermisos)
            .FirstOrDefaultAsync(r => r.Id == rolId);

        if (rol == null)
            return false;

        // Eliminar permisos actuales
        _context.RolPermisos.RemoveRange(rol.RolPermisos);

        // Añadir nuevos permisos
        foreach (var permisoId in permisosIds)
        {
            rol.RolPermisos.Add(new RolPermiso { RolId = rolId, PermisoId = permisoId });
        }

        await _context.SaveChangesAsync();
        return true;
    }
    /// <summary>
    /// Asigna un permiso específico a un rol, verificando primero si el permiso ya está asignado para evitar duplicados, si no existe la relación se crea una nueva entrada en la tabla de relación entre roles y permisos
    /// </summary>
    /// <param name="rolId"></param>
    /// <param name="permisoId"></param>
    /// <returns>True si se asignó correctamente, false en caso contrario</returns>
    public async Task<bool> AsignarPermisoAsync(int rolId, int permisoId)
    {
        var existe = await _context.RolPermisos.AnyAsync(rp =>
            rp.RolId == rolId && rp.PermisoId == permisoId
        );

        if (existe)
            return true;

        _context.RolPermisos.Add(new RolPermiso { RolId = rolId, PermisoId = permisoId });

        await _context.SaveChangesAsync();
        return true;
    }
    /// <summary>
    /// Quita un permiso específico de un rol, buscando la relación entre el rol y el permiso en la tabla de relación, si existe se elimina esa entrada para quitar el permiso del rol
    /// </summary>
    /// <param name="rolId"></param>
    /// <param name="permisoId"></param>
    /// <returns>True si se quitó correctamente, false en caso contrario</returns>
    public async Task<bool> QuitarPermisoAsync(int rolId, int permisoId)
    {
        var rp = await _context.RolPermisos.FirstOrDefaultAsync(rp =>
            rp.RolId == rolId && rp.PermisoId == permisoId
        );

        if (rp == null)
            return false;

        _context.RolPermisos.Remove(rp);
        await _context.SaveChangesAsync();
        return true;
    }
    /// <summary>
    /// Obtiene todos los roles disponibles en el sistema, devuelve una lista de objetos Rol que representan cada rol con sus propiedades, este método se utiliza para mostrar los roles disponibles en la interfaz de administración o panel de control (solo accesible para usuarios con permisos de administración)
    /// </summary>
    /// <returns>IEnumerable&lt;Rol&gt;</returns>
     public async Task<IEnumerable<Rol>> GetAllAsync()
    {
        return await _context.Roles.ToListAsync();
    }
}
