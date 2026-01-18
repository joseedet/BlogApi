using System.Security.Claims;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

/// <summary>
/// Controlador NotificacionesController
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificacionesController : ControllerBase
{
    /// <summary>
    /// INotificaciones Service _service
    /// </summary>
    private readonly INotificacionesService _service;

    /// <summary>
    /// Constructor NotificacionesController
    /// </summary>
    /// <param name="service"></param>
    public NotificacionesController(INotificacionesService service)
    {
        _service = service;
    }

    private int GetUsuarioId()
    {
        var claim = User.FindFirst("id") ?? User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
            throw new UnauthorizedAccessException("No se pudo obtener el id del usuario.");
        return int.Parse(claim.Value);
    }

    /// <summary>
    /// Obtiene notificaciones
    /// </summary>
    /// <param name="id"></param>
    /// <returns>IEnumerable de NotificacionDto</returns>
    // ------------------------------------------------------------
    // GET: api/notificaciones
    // ------------------------------------------------------------
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificacionDto>>> ObtenerNotificaciones()
    {
        int usuarioId = GetUsuarioId();
        var notificaciones = await _service.ObtenerPorUsuarioAsync(usuarioId);
        return Ok(notificaciones);
    }

    // ------------------------------------------------------------
    // GET: api/notificaciones/no-leidas
    // ------------------------------------------------------------

    /// <summary>
    /// Obtiene notificaciones no leidas
    /// </summary>
    /// <returns>IEnumerable de NotificacionDto<</returns>
    [HttpGet("no-leidas")]
    public async Task<ActionResult<IEnumerable<NotificacionDto>>> ObtenerNoLeidas()
    {
        int usuarioId = GetUsuarioId();
        var notificaciones = await _service.ObtenerNoLeidasAsync(usuarioId);
        return Ok(notificaciones);
    }

    // ------------------------------------------------------------
    // GET: api/notificaciones/paginadas?page=1&pageSize=10
    // ------------------------------------------------------------

    /// <summary>
    /// Número de páginas
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns>PaginacionResultado NotificacionDto/returns>
    [HttpGet("paginadas")]
    public async Task<ActionResult<PaginacionResultado<NotificacionDto>>> ObtenerPaginadas(
        int page = 1,
        int pageSize = 10
    )
    {
        int usuarioId = GetUsuarioId();
        var resultado = await _service.GetPaginadasAsync(usuarioId, page, pageSize);
        return Ok(resultado);
    }

    // ------------------------------------------------------------
    // PUT: api/notificaciones/{id}/leer
    // ------------------------------------------------------------

    /// <summary>
    /// Marcar como leida
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("{id}/leer")]
    public async Task<ActionResult> MarcarComoLeida(int id)
    {
        int usuarioId = GetUsuarioId();
        var ok = await _service.MarcarComoLeidaAsync(id, usuarioId);

        if (!ok)
            return NotFound(
                new { Message = "La notificación no existe o no pertenece al usuario." }
            );

        return NoContent();
    }

    // ------------------------------------------------------------
    // PUT: api/notificaciones/leer-todas
    // ------------------------------------------------------------

    /// <summary>
    /// Marcar todas las notificaciones como leídas
    /// </summary>
    /// <returns></returns>
    [HttpPut("leer-todas")]
    public async Task<ActionResult> MarcarTodasComoLeidas()
    {
        int usuarioId = GetUsuarioId();
        await _service.MarcarTodasComoLeidasAsync(usuarioId);
        return NoContent();
    }

    // ------------------------------------------------------------
    // DELETE: api/notificaciones/{id}
    // ------------------------------------------------------------

    /// <summary>
    /// Elimina la notificación
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Eliminar(int id)
    {
        int usuarioId = GetUsuarioId();
        var ok = await _service.EliminarAsync(id, usuarioId);

        if (!ok)
            return NotFound(
                new { Message = "La notificación no existe o no pertenece al usuario." }
            );

        return NoContent();
    }
}
