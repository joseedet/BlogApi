using BlogApi.DTO;

namespace BlogApi.Services;

public interface INotificacionesService
{
    public Task<List<NotificacionDto>> ObtenerNoLeidasAsync(int id);
    public Task MarcarTodasComoLeidasAsync(int id);
    public Task<PaginacionResultado<NotificacionDto>> GetPaginadasAsync(
        int userId,
        int page,
        int pageSize
    );
}
