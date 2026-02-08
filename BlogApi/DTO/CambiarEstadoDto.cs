using System;
using BlogApi.Utils.Enums;

namespace BlogApi.DTO;

/// <summary>
/// DTO para cambiar el estado de un comentario
/// </summary>
public class CambiarEstadoDto
{
    public ComentarioEstado Estado { get; set; }
}
