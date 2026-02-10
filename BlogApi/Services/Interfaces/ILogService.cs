using System;

namespace BlogApi.Services.Interfaces;
/// <summary>
/// Servicio para registrar logs administrativos, como bloqueos de usuarios, eliminación de contenido, etc.
/// </summary>
public interface ILogService
{
    /// <summary>
    /// Registra una acción administrativa en el sistema, guardando información como el ID del administrador que realizó la acción, la acción realizada, el ID del usuario afectado (si aplica) y detalles adicionales, este método se utiliza para mantener un historial de acciones administrativas para auditoría y seguimiento de cambios realizados por los administradores en el sistema
    /// </summary>
    /// <param name="adminId"></param>
    /// <param name="accion"></param>
    /// <param name="usuarioAfectadoId"></param>
    /// <param name="detalles"></param>
    /// <returns></returns>
    Task RegistrarAsync(
        int adminId,
        string accion,
        int? usuarioAfectadoId = null,
        string? detalles = null
    );
}
