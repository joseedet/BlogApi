using System;

namespace BlogApi.DTO;

public class DashboardDto
{
    public int TotalUsuarios { get; set; }
    public int UsuariosBloqueados { get; set; }
    public int UsuariosEmailVerificado { get; set; }
    public int TotalRoles { get; set; }
    public int TotalPermisos { get; set; }
    public List<RolUsoDto> RolesMasUsados { get; set; } = new();
    public List<PermisoUsoDto> PermisosMasUsados { get; set; } = new();
    public List<UsuarioDto> UltimosUsuarios { get; set; } = new();
    public List<LogDto> UltimosLogs { get; set; } = new();
}
