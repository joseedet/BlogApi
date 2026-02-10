using System;

namespace BlogApi.DTO;

public class PermisoDto
{
    public int Id { get; set; }
    public string Clave { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}
