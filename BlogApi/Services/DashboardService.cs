using System;
using BlogApi.Data;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

/// <summary>
/// Servicio para obtener datos del dashboard de administración, incluyendo estadísticas generales, roles y permisos más usados, últimos usuarios registrados y últimos logs de administración.
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly BlogDbContext _context;
    private readonly ICacheService _cacheService;
    private readonly ICacheConfigService _cacheConfigService;

    /// <summary>
    /// Constructor del servicio de dashboard, inyectando el contexto de la base de datos, el servicio de caché y el servicio de configuración de caché.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cacheService"></param>
    /// <param name="cacheConfigService"></param>
    public DashboardService(
        BlogDbContext context,
        ICacheService cacheService,
        ICacheConfigService cacheConfigService
    )
    {
        _context = context;
        _cacheService = cacheService;
        _cacheConfigService = cacheConfigService;
    }

    /// <summary>
    /// Obtiene los datos para el dashboard de administración, incluyendo estadísticas generales, roles y permisos más usados, últimos usuarios registrados y últimos logs de administración. Utiliza caché para mejorar el rendimiento, con una duración configurable. Requiere el permiso "Dashboard.Ver".
    /// </summary>
    /// <returns>Un objeto DashboardDto con los datos del dashboard.</returns>
    public async Task<DashboardDto> ObtenerDashboardAsync()
    {
        var config = await _cacheConfigService.ObtenerConfigAsync();

        return await _cacheService.GetOrSetAsync(
            "dashboard_general",
            async () =>
            {
                var dto = new DashboardDto();

                dto.TotalUsuarios = await _context.Usuarios.CountAsync();
                dto.UsuariosBloqueados = await _context.Usuarios.CountAsync(u => u.EstaBloqueado);
                dto.UsuariosEmailVerificado = await _context.Usuarios.CountAsync(u =>
                    u.EmailVerificado
                );

                dto.TotalRoles = await _context.Roles.CountAsync();
                dto.TotalPermisos = await _context.Permisos.CountAsync();

                dto.RolesMasUsados = await _context
                    .Roles.Select(r => new RolUsoDto
                    {
                        Nombre = r.Nombre,
                        CantidadUsuarios = r.UsuarioRoles.Count,
                    })
                    .OrderByDescending(r => r.CantidadUsuarios)
                    .Take(5)
                    .ToListAsync();

                dto.PermisosMasUsados = await _context
                    .Permisos.Select(p => new PermisoUsoDto
                    {
                        Clave = p.Clave,
                        CantidadRoles = p.RolPermisos.Count,
                    })
                    .OrderByDescending(p => p.CantidadRoles)
                    .Take(5)
                    .ToListAsync();

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
            },
            TimeSpan.FromSeconds(config.ExpiracionDashboardSegundos)
        );
    }
}
