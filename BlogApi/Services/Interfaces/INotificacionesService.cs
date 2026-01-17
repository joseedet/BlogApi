using BlogApi.DTO;
using BlogApi.Models;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Interfaz INotificacionesService
/// </summary>
public interface INotificacionesService
{
    /// <summary>
    /// Obtenemos notificaciones no leidas
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<List<NotificacionDto>> ObtenerNoLeidasAsync(int id);

    /// <summary>
    /// Marcamos todas las notiicaciones com leídas
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task MarcarTodasComoLeidasAsync(int id);
    public Task<PaginacionResultado<NotificacionDto>> GetPaginadasAsync(
        int userId,
        int page,
        int pageSize
    );

    /// <summary>
    /// Creamos notificación
    /// </summary>
    /// <param name="notificacion"></param>
    /// <returns></returns>
    Task CrearAsync(Notificacion notificacion);

}
