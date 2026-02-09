using System;

namespace BlogApi.Services.Interfaces;
/// <summary>
/// Servicio para registrar logs administrativos, como bloqueos de usuarios, eliminación de contenido, etc.
/// </summary>
public interface ILogService
{
    Task RegistrarAsync(
        int adminId,
        string accion,
        int? usuarioAfectadoId = null,
        string? detalles = null
    );
}
