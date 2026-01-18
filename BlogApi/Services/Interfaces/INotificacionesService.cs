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

    /// <summary>
    /// Obtiene IEnumerable de NotificacionDto
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns>IEnumerable de NotificacionesDto</returns>
    public Task<IEnumerable<NotificacionDto>> ObtenerPorUsuarioAsync(int usuarioId);

    /// <summary>
    /// Marca las notificaciones como leídas
    /// </summary>
    /// <param name="id"></param>
    /// <param name="usuarioId"></param>
    /// <returns>Verdadero si ha marcado como leidas en caso contrario falso</returns>
    public Task<bool> MarcarComoLeidaAsync(int id, int usuarioId);

    /// <summary>
    /// Elimina notificacion
    /// </summary>
    /// <param name="id"></param>
    /// <param name="usuarioId"></param>
    /// <returns>Verdadero si ha sido eliminada en caso contrario falso.</returns>
    public Task<bool> EliminarAsync(int id, int usuarioId);
}
