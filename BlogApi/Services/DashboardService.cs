using System;
using BlogApi.Data;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

public class DashboardService : IDashboardService
{
    private readonly BlogDbContext _context;

    public DashboardService(BlogDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> ObtenerDashboardAsync()
    {
        var dto = new DashboardDto();

        // Usuarios
        dto.TotalUsuarios = await _context.Usuarios.CountAsync();
        dto.UsuariosBloqueados = await _context.Usuarios.CountAsync(u => u.EstaBloqueado);
        dto.UsuariosEmailVerificado = await _context.Usuarios.CountAsync(u => u.EmailVerificado);

        // Roles y permisos
        dto.TotalRoles = await _context.Roles.CountAsync();
        dto.TotalPermisos = await _context.Permisos.CountAsync();

        // Roles más usados
        dto.RolesMasUsados = await _context
            .Roles.Select(r => new RolUsoDto
            {
                Nombre = r.Nombre,
                CantidadUsuarios = r.UsuarioRoles.Count,
            })
            .OrderByDescending(r => r.CantidadUsuarios)
            .Take(5)
            .ToListAsync();

        // Permisos más usados
        dto.PermisosMasUsados = await _context
            .Permisos.Select(p => new PermisoUsoDto
            {
                Clave = p.Clave,
                CantidadRoles = p.RolPermisos.Count,
            })
            .OrderByDescending(p => p.CantidadRoles)
            .Take(5)
            .ToListAsync();

        // Últimos usuarios registrados
        dto.UltimosUsuarios = await _context
            .Usuarios.OrderByDescending(u => u.Id)
            .Take(5)
            .Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Apellidos = u.Apellidos,
                Email = u.Email,
                EstaBloqueado = u.EstaBloqueado,
                EmailVerificado = u.EmailVerificado,
                AvatarUrl = u.AvatarUrl,
                Roles = u.UsuarioRoles.Select(ur => ur.Rol.Nombre).ToList(),
            })
            .ToListAsync();

        // Últimos logs
        dto.UltimosLogs = await _context
            .LogAdmins.OrderByDescending(l => l.Id)
            .Take(10)
            .Select(l => new LogDto
            {
                Id = l.Id,
                UsuarioId = l.UsuarioAdminId,
                Accion = l.Accion,
                Fecha = l.Fecha,
                Detalles = l.Detalles,
            })
            .ToListAsync();

        return dto;
    }
}
