using System.Security.Claims;
using BlogApi.Domain.Factories;
using BlogApi.DTO;
using BlogApi.Mapper;
using BlogApi.Models;
using BlogApi.Services.Interfaces;
using BlogApi.Utils.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controlador para gestionar comentarios en los posts del blog.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ComentariosController : ControllerBase
{
    private readonly IComentarioService _service;
    private readonly IPostService _postService;
    private readonly INotificacionesService _notificaciones;

    /// <summary>
    /// Constructor del controlador de comentarios, inyecta los servicios necesarios para gestionar comentarios, posts y notificaciones.
    /// </summary>
    /// <param name="service"></param>
    /// <param name="postService"></param>
    /// <param name="notificaciones"></param>
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
    //[Authorize(Roles = "Administrador,Editor,Autor")]
    [HttpGet("post/{postId}")]
    public async Task<IActionResult> GetByPost(int postId)
    {
        var comentarios = await _service.GetComentariosDePostAsync(postId);
        return Ok(comentarios.Select(c => c.ToDto()));
    }

    // Crear comentario o respuesta
    //[Authorize(Roles = "Administrador,Editor,Autor,Suscriptor")]
    [Authorize(Policy = "Permiso:Comentarios.Crear")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateComentarioDto dto)
    {
        var post = await _postService.GetByIdAsync(dto.PostId);
        if (string.IsNullOrWhiteSpace(dto.Contenido))
            return BadRequest("El contenido del comentario no puede estar vacío.");
        if (post == null)
            return NotFound("El post no existe.");
        // Usuario autenticado
        int usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Validar comentario padre
        if (dto.ComentarioPadreId.HasValue)
        {
            var padre = await _service.GetByIdAsync(dto.ComentarioPadreId.Value);
            if (padre == null)
                return BadRequest("El comentario padre no existe.");
            if (padre.PostId != dto.PostId)
                return BadRequest("El comentario padre no pertenece a este post.");
        }
        var comentario = new Comentario
        {
            Contenido = dto.Contenido,
            PostId = dto.PostId,
            UsuarioId = usuarioId,
            ComentarioPadreId = dto.ComentarioPadreId,
        };
        var created = await _service.CrearComentarioAsync(comentario);
        // Notificación si es respuesta
        if (dto.ComentarioPadreId.HasValue)
        {
            var padre = await _service.GetByIdAsync(dto.ComentarioPadreId.Value);
            if (padre != null && padre.UsuarioId != usuarioId)
            {
                var notificacion = NotificacionFactory.RespuestaComentario(
                    usuarioDestinoId: padre.UsuarioId!.Value,
                    usuarioOrigenId: usuarioId,
                    postId: dto.PostId,
                    comentarioId: padre.Id,
                    contenido: created.Contenido,
                    autorComentario: User.FindFirstValue(ClaimTypes.Name)
                );
                await _notificaciones.CrearAsync(notificacion);
            }
        }
        return Ok(created.ToDto());
    }

    // Eliminar comentario
    [Authorize(Policy = "Permiso:Comentarios.Eliminar")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        int usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var comentario = await _service.GetByIdAsync(id);
        if (comentario == null)
            return NotFound();
        var ok = await _service.EliminarComentarioAsync(id, usuarioId);
        if (!ok)
            return Forbid();
        // o NotFound, según tu lógica
        return NoContent();
    }

    // Cambiar estado (solo Admin/Editor)
    [Authorize(Policy = "Permiso:Comentarios.Moderar")]
    [HttpPatch("{id}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoDto dto)
    {
        int usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var ok = await _service.CambiarEstadoAsync(id, usuarioId, dto.Estado);
        if (!ok)
            return NotFound();
        return NoContent();
    }

    // Obtener comentarios por estado
    //[Authorize(Roles = "Administrador,Editor")]
    [Authorize(Policy = "Permiso:Comentarios.Moderar")]
    [HttpGet("estado/{estado}")]
    public async Task<IActionResult> GetByEstado(string estado)
    {
        var comentarios = await _service.GetByEstadoAsync(estado);
        return Ok(comentarios.Select(c => c.ToDto()));
    }

    // Aprobar comentario
    [Authorize(Policy = "Permiso:Comentarios.Moderar")]
    [HttpPatch("{id}/aprobar")]
    public async Task<IActionResult> Aprobar(int id)
    {
        int usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        ;
        var ok = await _service.CambiarEstadoAsync(id, usuarioId, ComentarioEstado.Aprobado);
        if (!ok)
            return NotFound();
        return NoContent();
    }

    // Rechazar comentario
    [Authorize(Policy = "Permiso:Comentarios.Moderar")]
    [HttpPatch("{id}/rechazar")]
    public async Task<IActionResult> Rechazar(int id)
    {
        int usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var ok = await _service.CambiarEstadoAsync(id, usuarioId, ComentarioEstado.Rechazado);
        if (!ok)
            return NotFound();
        return NoContent();
    }

    // Comentarios pendientes paginados
    [Authorize(Policy = "Permiso:Comentarios.Moderar")]
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
