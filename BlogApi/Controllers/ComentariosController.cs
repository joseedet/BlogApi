using System.Security.Claims;
using BlogApi.Domain.Factories;
using BlogApi.DTO;
using BlogApi.Mapper;
using BlogApi.Models;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ComentariosController : ControllerBase
{
    private readonly IComentarioService _service;
    private readonly IPostService _postService;
    private readonly INotificacionesService _notificaciones;

    public ComentariosController(
        IComentarioService service,
        IPostService postService,
        INotificacionesService notificaciones
    )
    {
        _service = service;
        _postService = postService;
        _notificaciones = notificaciones;
    }

    // Obtener comentarios raíz de un post
    [Authorize(Roles = "Administrador,Editor,Autor")]
    [HttpGet("post/{postId}")]
    public async Task<IActionResult> GetByPost(int postId)
    {
        var comentarios = await _service.GetComentariosDePostAsync(postId);
        return Ok(comentarios.Select(c => c.ToDto()));
    }

    // Crear comentario o respuesta
    [Authorize(Roles = "Administrador,Editor,Autor,Suscriptor")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateComentarioDto dto)
    {
        var post = await _postService.GetByIdAsync(dto.PostId);
        if (string.IsNullOrWhiteSpace(dto.Contenido))
            return BadRequest("El contenido del comentario no puede estar vacío.");
        if (post == null)
            return NotFound("El post no existe.");
        // Usuario autenticado
        var userId = int.Parse(User.FindFirst("id")!.Value);

        var comentario = new Comentario
        {
            Contenido = dto.Contenido,
            PostId = dto.PostId,
            UsuarioId = userId, // ← Seguridad corregida
            ComentarioPadreId = dto.ComentarioPadreId,
        };

        var created = await _service.CrearComentarioAsync(comentario);

        // Notificación si es respuesta a un comentario
        if (dto.ComentarioPadreId != null)
        {
            var comentarioPadre = await _service.GetByIdAsync(dto.ComentarioPadreId.Value);
            var autorComentario = User.FindFirstValue(ClaimTypes.Name);
            var postId = dto.PostId;

            if (comentarioPadre != null && comentarioPadre.UsuarioId != null)
            {
                var notificacion = NotificacionFactory.RespuestaComentario(
                    usuarioDestinoId: comentario.UsuarioId.Value,
                    usuarioOrigenId: comentarioPadre.UsuarioId.Value,
                    postId: postId,
                    comentarioId: comentarioPadre.Id,
                    contenido: created.Contenido,
                    autorComentario: autorComentario
                );

                await _notificaciones.CrearAsync(notificacion);
            }
        }

        return Ok(created.ToDto());
    }

    // Eliminar comentario
    [Authorize(Roles = "Administrador,Editor")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var usuarioId))
            return Unauthorized();

        bool esAdmin = User.IsInRole("Administrador");
        bool esEditor = User.IsInRole("Editor");

        var ok = await _service.EliminarComentarioAsync(id, usuarioId, esAdmin || esEditor);
        if (!ok)
            return Forbid();

        return NoContent();
    }

    // Cambiar estado (solo Admin/Editor)
    [Authorize(Roles = "Administrador,Editor")]
    [HttpPatch("{id}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] string estado)
    {
        var estadosValidos = new[] { "Pendiente", "Aprobado", "Rechazado" };
        if (!estadosValidos.Contains(estado))
            return BadRequest("Estado inválido.");

        var ok = await _service.CambiarEstadoAsync(id, estado);
        if (!ok)
            return NotFound();

        return NoContent();
    }

    // Obtener comentarios por estado
    [Authorize(Roles = "Administrador,Editor")]
    [HttpGet("estado/{estado}")]
    public async Task<IActionResult> GetByEstado(string estado)
    {
        var comentarios = await _service.GetByEstadoAsync(estado);
        return Ok(comentarios.Select(c => c.ToDto()));
    }

    // Aprobar comentario
    [Authorize(Roles = "Administrador,Editor")]
    [HttpPatch("{id}/aprobar")]
    public async Task<IActionResult> Aprobar(int id)
    {
        var ok = await _service.CambiarEstadoAsync(id, "Aprobado");
        if (!ok)
            return NotFound();

        return NoContent();
    }

    // Rechazar comentario
    [Authorize(Roles = "Administrador,Editor")]
    [HttpPatch("{id}/rechazar")]
    public async Task<IActionResult> Rechazar(int id)
    {
        var ok = await _service.CambiarEstadoAsync(id, "Rechazado");
        if (!ok)
            return NotFound();

        return NoContent();
    }

    // Comentarios pendientes paginados
    [Authorize(Roles = "Administrador,Editor")]
    [HttpGet("pendientes")]
    public async Task<IActionResult> GetPendientes(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20
    )
    {
        var result = await _service.GetPendientesPaginadoAsync(page, pageSize);
        return Ok(result);
    }
}
