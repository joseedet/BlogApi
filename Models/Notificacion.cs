using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Utils;

namespace BlogApi.Models;

public class Notificacion
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public  Usuario Usuario { get; set; }= null;

    public required string Mensaje { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public bool Leida { get; set; } = false;

    public TipoNotificacion Tipo { get; set; }

    // Opcional: datos adicionales (JSON)
    public string? Payload { get; set; }
}
