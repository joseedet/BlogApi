using System;

namespace BlogApi.DTO;

public class RolDetalleDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public List<PermisoDto> Permisos { get; set; } = new();
    public List<UsuarioDto> Usuarios { get; set; } = new();
}
