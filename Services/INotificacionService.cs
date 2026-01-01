using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Models;

namespace BlogApi.Services;

public interface INotificacionService
{
    Task CrearAsync(int usuarioId, string mensaje);
    Task<IEnumerable<Notificacion>> GetByUsuarioAsync(int usuarioId);
    Task<bool> MarcarComoLeidaAsync(int id, int usuarioId);
}
