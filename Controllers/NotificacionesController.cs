using System.Security.Claims;
using BlogApi.DTO;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificacionesController : ControllerBase
{
    private readonly INotificacionService _service;

    /// <summary>
    /// Constructor de NotificacionesController
    /// </summary>
    /// <param name="service"></param>
    public NotificacionesController(INotificacionService service)
    {
        _service = service;
    }

    /// <summary>
    /// Obtiene las notificaciones del usuario autenticado
    /// </summary>
    /// <returns>Lista de notificaciones</returns>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var notificaciones = await _service.GetByUsuarioAsync(userId);
        return Ok(notificaciones);
    }

    /// <summary>
    /// Marca una notificación como leída
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPatch("{id}/leer")]
    [Authorize]
    public async Task<IActionResult> MarcarComoLeida(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var ok = await _service.MarcarComoLeidaAsync(id, userId);
        if (!ok)
            return NotFound();
        return NoContent();
    }
}
