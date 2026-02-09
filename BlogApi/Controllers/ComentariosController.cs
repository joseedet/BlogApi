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

    /// <summary>
    ///     Crea un nuevo comentario o respuesta. El usuario debe estar autenticado y tener permiso para crear comentarios. Si se especifica un comentario padre, se valida que exista y que pertenezca al mismo post. Si el comentario es una respuesta, se crea una notificación para el autor del comentario padre (si no es el mismo usuario que crea la respuesta).
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Elimina un comentario por su ID. Solo el autor del comentario o usuarios con permiso de eliminar comentarios pueden eliminarlo. Si el comentario tiene respuestas, se puede optar por eliminar solo el comentario o toda la rama de respuestas.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Cambia el estado de un comentario (Aprobado, Rechazado, Pendiente). Solo accesible para usuarios con permiso de moderar comentarios.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Obtiene comentarios por estado. Solo accesible para usuarios con permiso de moderar comentarios.
    ///
    /// </summary>
    /// <param name="estado"></param>
    /// <returns></returns>
    // Obtener comentarios por estado
    //[Authorize(Roles = "Administrador,Editor")]
    [Authorize(Policy = "Permiso:Comentarios.Moderar")]
    [HttpGet("estado/{estado}")]
    public async Task<IActionResult> GetByEstado(string estado)
    {
        var comentarios = await _service.GetByEstadoAsync(estado);
        return Ok(comentarios.Select(c => c.ToDto()));
    }

    /// <summary>
    ///  Aprobar comentario. Cambia el estado del comentario a "Aprobado". Solo accesible para usuarios con permiso de moderar comentarios.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Rechazar comentario. Cambia el estado del comentario a "Rechazado". Solo accesible para usuarios con permiso de moderar comentarios.
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
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

    /// <summary>
    ///     Obtiene comentarios pendientes paginados. Solo accesible para usuarios con permiso de moderar comentarios.
    ///
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Filtra comentarios según los criterios especificados en el DTO de filtro. Permite filtrar por ID de post, ID de usuario y estado del comentario, así como paginar los resultados.
    /// </summary>
    /// <param name="filtro"></param>
    /// <returns></returns>
    [Authorize(Policy = "Permiso:Comentarios.Moderar")]
    [HttpGet("filtrar")]
    public async Task<IActionResult> Filtrar([FromQuery] ComentarioFiltroDto filtro)
    {
        var result = await _service.FiltrarAsync(filtro);

        return Ok(
            new
            {
                result.PaginaActual,
                result.TotalPaginas,
                result.TotalRegistros,
                Comentarios = result.Items.Select(c => c.ToDto()),
            }
        );
    }
}
