using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Models;

namespace BlogApi.Services;

[Obsolete("INotificacionService está obsoleto. Usa INotificacionesService en su lugar.")]
public interface INotificacionService
{
    Task CrearAsync(int usuarioId, string mensaje);
    Task<IEnumerable<Notificacion>> GetByUsuarioAsync(int usuarioId);
    Task<bool> MarcarComoLeidaAsync(int id, int usuarioId);
}
