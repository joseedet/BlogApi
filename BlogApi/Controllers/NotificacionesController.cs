using BlogApi.DTO;
using BlogApi.Repositories;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

/// <summary>
/// Controlador para gestionar notificaciones
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificacionesController : ControllerBase
{
    /// <summary>
    /// Servicio de notificaciones
    /// </summary>
    [Obsolete("NotificacionService está obsoleto. Usa NotificacionesService en su lugar.")]
    private readonly INotificacionService _notificacionService;
    private readonly INotificacionRepository _notificacionRepository;

    /// <summary>
    /// Constructor de NotificacionesController
    /// </summary>
    /// <param name="notificacionService"></param>
    /// <param name="notificacionRepository"></param>
    [Obsolete("NotificacionService está obsoleto. Usa NotificacionesService en su lugar.")]
    public NotificacionesController(
        INotificacionService notificacionService,
        INotificacionRepository notificacionRepository
    )
    {
        _notificacionService = notificacionService;
        _notificacionRepository = notificacionRepository;
    }

    /// <summary>
    ///     Obtiene el ID del usuario autenticado
    /// </summary>
    /// <returns></returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    private int GetUsuarioId()
    {
        var claim = User.FindFirst("id") ?? User.FindFirst("sub");
        if (claim == null)
            throw new UnauthorizedAccessException("No se pudo obtener el id del usuario.");
        return int.Parse(claim.Value);
    }

    /// <summary>
    ///     Obtiene todas las notificaciones del usuario autenticado
    /// </summary>
    /// <returns>IEnumerable de NotificacionDto</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificacionDto>>> ObtenerNotificaciones()
    {
        try
        {
            int usuarioId = GetUsuarioId();

            var notificaciones = await _notificacionRepository.ObtenerPorUsuarioAsync(usuarioId);

            var dto = notificaciones.Select(n => new NotificacionDto
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
            });

            return Ok(dto);
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new { Message = "Error al obtener las notificaciones.", Error = ex.Message }
            );
        }
    }

    /// <summary>
    ///   Marca una notificación como leída
    /// </summary>
    /// <param name="id"></param>
    /// <returns>ActionResult</returns>
    [HttpPut("{id}/leer")]
    public async Task<ActionResult> MarcarComoLeida(int id)
    {
        try
        {
            int usuarioId = GetUsuarioId();

            var notificacion = await _notificacionRepository.ObtenerPorIdAsync(id);

            if (notificacion == null)
                return NotFound(new { Message = "La notificación no existe." });

            if (notificacion.UsuarioDestinoId != usuarioId)
                return Forbid("No puedes modificar notificaciones de otro usuario.");

            await _notificacionRepository.MarcarComoLeidaAsync(id);

            return Ok(new { Message = "Notificación marcada como leída." });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new { Message = "Error al marcar la notificación.", Error = ex.Message }
            );
        }
    }

    /// <summary>
    ///     Marca todas las notificaciones del usuario autenticado como leídas
    /// </summary>
    /// <returns>ActionResult</returns>
    [HttpPut("leer-todas")]
    public async Task<ActionResult> MarcarTodasComoLeidas()
    {
        try
        {
            int usuarioId = GetUsuarioId();

            await _notificacionService.MarcarTodasComoLeidasAsync(usuarioId);

            return Ok(new { Message = "Todas las notificaciones han sido marcadas como leídas." });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new { Message = "Error al marcar todas las notificaciones.", Error = ex.Message }
            );
        }
    }

    /// <summary>
    ///     Obtiene las notificaciones no leídas del usuario autenticado con paginación
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns>PaginacionResultado de NotificacionDto</returns>
    [HttpGet("no-leidas")]
    public async Task<ActionResult<PaginacionResultado<NotificacionDto>>> ObtenerNoLeidasPaginadas(
        int page = 1,
        int pageSize = 10
    )
    {
        try
        {
            int usuarioId = GetUsuarioId();

            var resultado = await _notificacionRepository.ObtenerNoLeidasPaginadasAsync(
                usuarioId,
                page,
                pageSize
            );

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new
                {
                    Message = "Error al obtener las notificaciones no leídas.",
                    Error = ex.Message,
                }
            );
        }
    }
}
