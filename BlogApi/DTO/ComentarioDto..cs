using BlogApi.DTO;

public class ComentarioDto
{
    public int Id { get; set; }
    public string Contenido { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }

    // Enum como texto ("Pendiente", "Aprobado", "Rechazado")
    public string Estado { get; set; } = string.Empty;

    // Enum como número (0, 1, 2)
    public int EstadoId { get; set; }

    public UsuarioDto? Usuario { get; set; }
    public List<ComentarioDto> Respuestas { get; set; } = new();
}
