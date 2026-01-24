using System.Security.Claims;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Todas requieren usuario autenticado
public class NotificacionesController : ControllerBase
{
    private readonly INotificacionesService _service;

    public NotificacionesController(INotificacionesService service)
    {
        _service = service;
    }

    // ------------------------------------------------------------
    // Obtener notificaciones paginadas
    // ------------------------------------------------------------
    [HttpGet]
    public async Task<IActionResult> GetPaginadas(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20
    )
    {
        var userId = int.Parse(User.FindFirst("id")!.Value);

        var result = await _service.GetPaginadasAsync(userId, page, pageSize);

        return Ok(result);
    }

    // ------------------------------------------------------------
    // Obtener no leídas
    // ------------------------------------------------------------
    [HttpGet("no-leidas")]
    public async Task<IActionResult> GetNoLeidas()
    {
        var userId = int.Parse(User.FindFirst("id")!.Value);

        // Reutilizamos la paginación para obtener todas y filtramos
        var result = await _service.GetPaginadasAsync(userId, 1, int.MaxValue);

        var noLeidas = result.Items.Where(n => !n.Leida);

        return Ok(noLeidas);
    }

    // ------------------------------------------------------------
    // Marcar una notificación como leída
    // ------------------------------------------------------------
    [HttpPatch("{id}/leer")]
    public async Task<IActionResult> MarcarComoLeida(int id)
    {
        var userId = int.Parse(User.FindFirst("id")!.Value);

        var ok = await _service.MarcarComoLeidaAsync(id, userId);

        if (!ok)
            return Forbid(); // No pertenece al usuario o no existe

        return NoContent();
    }

    // ------------------------------------------------------------
    // Marcar todas como leídas
    // ------------------------------------------------------------
    [HttpPatch("leer-todas")]
    public async Task<IActionResult> MarcarTodas()
    {
        var userId = int.Parse(User.FindFirst("id")!.Value);

        await _service.MarcarTodasComoLeidasAsync(userId);

        return NoContent();
    }

    // ------------------------------------------------------------
    // Eliminar notificación
    // ------------------------------------------------------------
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = int.Parse(User.FindFirst("id")!.Value);

        var ok = await _service.EliminarAsync(id, userId);

        if (!ok)
            return Forbid();

        return NoContent();
    }
}
