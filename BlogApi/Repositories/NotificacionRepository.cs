using BlogApi.Data;
using BlogApi.DTO;
using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repositories;

/// <summary>
/// Repositorio para gestionar notificaciones
/// </summary>
public class NotificacionRepository : INotificacionRepository
{
    private readonly BlogDbContext _context;

    /// <summary>
    /// Constructor de NotificacionRepository
    /// </summary>
    /// <param name="context"></param>
    public NotificacionRepository(BlogDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Crea una nueva notificación
    /// </summary>
    /// <param name="notificacion"></param>
    /// <returns>Task</returns>
    public async Task CrearAsync(Notificacion notificacion)
    {
        _context.Notificaciones.Add(notificacion);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Obtiene una notificación por su ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Notificacion</returns>
    public async Task<Notificacion?> ObtenerPorIdAsync(int id)
    {
        return await _context.Notificaciones.FirstOrDefaultAsync(n => n.Id == id);
    }

    /// <summary>
    /// Obtiene todas las notificaciones de un usuario
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns>IEnumerable de notificaciones</returns>
    public async Task<IEnumerable<Notificacion>> ObtenerPorUsuarioAsync(int usuarioId)
    {
        return await _context
            .Notificaciones.Where(n => n.UsuarioDestinoId == usuarioId)
            .OrderByDescending(n => n.Fecha)
            .ToListAsync();
    }

    /// <summary>
    /// Marca una notificación como leída
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Task</returns>
    public async Task MarcarComoLeidaAsync(int id)
    {
        var notificacion = await _context.Notificaciones.FindAsync(id);
        if (notificacion != null)
        {
            notificacion.Leida = true;
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Obtiene las notificaciones no leídas de un usuario con paginación
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns>PaginacionResultado de NotificacionDto</returns>
    public async Task<PaginacionResultado<NotificacionDto>> ObtenerNoLeidasPaginadasAsync(
        int usuarioId,
        int page,
        int pageSize
    )
    {
        var query = _context
            .Notificaciones.Where(n => n.UsuarioDestinoId == usuarioId && !n.Leida)
            .OrderByDescending(n => n.Fecha);

        var total = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new NotificacionDto
            {
                Id = n.Id,
                UsuarioDestinoId = n.UsuarioDestinoId,
                UsuarioOrigenId = n.UsuarioOrigenId,
                Tipo = n.Tipo,
                PostId = n.PostId,
                ComentarioId = n.ComentarioId,
                Mensaje = n.Mensaje,
                Fecha = n.Fecha,
                Leida = n.Leida,
                Payload = n.Payload,
            })
            .ToListAsync();

        return new PaginacionResultado<NotificacionDto>
        {
            Items = items,
            PaginaActual = page,
            TotalPaginas = (int)Math.Ceiling(total / (double)pageSize),
            TotalRegistros = total,
        };
    }

    /// <summary>
    /// Marca todas las notificaciones de un usuario como leídas
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Task</returns>
    public async Task<Notificacion?> GetByIdAsync(int id)
    {
        return await _context.Notificaciones.FirstOrDefaultAsync(n => n.Id == id);
    }

    /// <summary>
    /// Elimina una notificación
    /// </summary>
    /// <param name="notificacion"></param>
    /// <returns>Task</returns>
    public async Task EliminarAsync(Notificacion notificacion)
    {
        _context.Notificaciones.Remove(notificacion);
        await Task.CompletedTask;
    }
    /// <summary>
    /// Marca todas las notificaciones como leidas
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task MarcarTodasComoLeidasAsync(int usuarioId)
    {
        throw new NotImplementedException();
    }
}
