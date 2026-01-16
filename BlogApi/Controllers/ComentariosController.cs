using System.Security.Claims;
using BlogApi.Domain.Factories;
using BlogApi.DTO;
using BlogApi.Mapper;
using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controlador para gestionar comentarios en el blog.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ComentariosController : ControllerBase
{
    /// <summary>
    /// Servicio de comentarios.
    /// </summary>
    private readonly IComentarioService _service;

    /// <summary>
    /// Servicio de posts.
    /// </summary>
    private readonly IPostService _postService;

    /// <summary>
    /// Servicio de notificaciones.
    /// </summary>
    private readonly INotificacionesService _notificaciones;

    /// <summary>
    /// Constructor del controlador de comentarios.
    /// </summary>
    /// <param name="service"></param>
    /// <param name="postService"></param>
    /// <param name="notificaciones"></param>
    /// <returns></returns>
    /// </summary>
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

    //Obtener comentarios raíz de un post
    /// <summary>
    /// Obtiene los comentarios raíz de un post específico.
    /// </summary>
    /// <param name="postId"></param>
    /// <returns></returns>
    /// </summary>
    [Authorize(Roles = "Administrador,Editor,Autor")]
    [HttpGet("post/{postId}")]
    public async Task<IActionResult> GetByPost(int postId)
    {
        var comentarios = await _service.GetComentariosDePostAsync(postId);
        return Ok(comentarios.Select(c => c.ToDto()));
    }

    // Crear comentario o respuesta
    /// <summary>
    /// Crea un nuevo comentario o respuesta a un comentario existente.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// </summary>
    [Authorize(Roles = "Administrador,Editor,Autor,Suscriptor")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateComentarioDto dto)
    {
        var comentario = new Comentario
        {
            Contenido = dto.Contenido,
            PostId = dto.PostId,
            UsuarioId = dto.UsuarioId,
            ComentarioPadreId = dto.ComentarioPadreId,
        };
        var created = await _service.CrearComentarioAsync(comentario);

        // 🔥 Notificación si es respuesta a un comentario
        if (dto.ComentarioPadreId != null)
        {
            var comentarioPadre = await _service.GetByIdAsync(dto.ComentarioPadreId.Value);
            var notificacion = NotificacionFactory.RespuestaComentario(
                usuarioId: comentarioPadre.UsuarioId.Value, // autor del comentario original
                comentarioId: comentarioPadre.Id,
                contenido: created.Contenido
            );
            await _notificaciones.CrearAsync(notificacion);
        }
        return Ok(created.ToDto());
    }

    // Eliminar comentario
    /// <summary>
    /// Elimina un comentario específico.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// </summary>
    [Authorize(Roles = "Administrador,Editor")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        // Obtener ID del usuario autenticado var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var usuarioId)) return Unauthorized(); // Roles elevados bool esAdmin = User.IsInRole("Administrador"); bool esEditor = User.IsInRole("Editor"); var ok = await _service.EliminarComentarioAsync(id, usuarioId, esAdmin || esEditor); if (!ok) return Forbid(); // o NotFound según tu lógica return NoContent();
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var usuarioId))
            return Unauthorized();

        // Roles elevados
        bool esAdmin = User.IsInRole("Administrador");
        bool esEditor = User.IsInRole("Editor");

        var ok = await _service.EliminarComentarioAsync(id, usuarioId, esAdmin || esEditor);
        if (!ok)
            return Forbid(); // o NotFound según tu lógica

        return NoContent();
    }

    // Moderar comentario (solo Admin/Editor)
    /// <summary>
    /// Cambia el estado de un comentario específico.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="estado"></param>
    /// <returns></returns>
    /// </summary>
    [Authorize(Roles = "Administrador,Editor")]
    [HttpPatch("{id}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] string estado)
    {
        var ok = await _service.CambiarEstadoAsync(id, estado);
        if (!ok)
            return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Obtiene comentarios por estado.
    /// </summary>
    /// <param name="estado"></param>
    /// <returns></returns>
    /// </summary>
    [HttpGet("estado/{estado}")]
    [Authorize(Roles = "Administrador,Editor")]
    public async Task<IActionResult> GetByEstado(string estado)
    {
        var comentarios = await _service.GetByEstadoAsync(estado);
        return Ok(comentarios.Select(c => c.ToDto()));
    }
}
