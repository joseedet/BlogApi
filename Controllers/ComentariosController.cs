using System.Security.Claims;
using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ComentariosController : ControllerBase
{
    private readonly IComentarioService _service;

    public ComentariosController(IComentarioService service)
    {
        _service = service;
    }

    //Obtener comentarios raíz de un post
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
        var comentario = new Comentario
        {
            Contenido = dto.Contenido,
            PostId = dto.PostId,
            UsuarioId = dto.UsuarioId,
            ComentarioPadreId = dto.ComentarioPadreId,
        };
        var created = await _service.CrearComentarioAsync(comentario);
        return Ok(created.ToDto());
    }

    // Eliminar comentario
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

    // Moderación
    [Authorize(Roles = "Administrador,Editor")]
    [HttpPatch("{id}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] string estado)
    {
        var ok = await _service.CambiarEstadoAsync(id, estado);
        if (!ok)
            return NotFound();
        return NoContent();
    }
}
