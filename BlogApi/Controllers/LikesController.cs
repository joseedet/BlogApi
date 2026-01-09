using BlogApi.DTO;
using BlogApi.Hubs;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requiere JWT
public class LikesController : ControllerBase
{
    private readonly ILikeService _likeService;
    private readonly IPostRepository _postRepository;
    private readonly IComentarioRepository _comentarioRepository;
    private readonly INotificacionService _notificacionService;
    private readonly IHubContext<NotificacionesHub> _notificacionesHub;

    public LikesController(
        ILikeService likeService,
        IPostRepository postRepository,
        IComentarioRepository comentarioRepository,
        INotificacionService notificacionService,
        IHubContext<NotificacionesHub> notificacionesHub
    )
    {
        _likeService = likeService;
        _postRepository = postRepository;
        _comentarioRepository = comentarioRepository;
        _notificacionService = notificacionService;
        _notificacionesHub = notificacionesHub;
    }

    // ---------------------------------------------------------
    //  Helpers
    // ---------------------------------------------------------

    private int GetUsuarioIdFromToken()
    {
        var claim = User.FindFirst("id") ?? User.FindFirst("sub");
        if (claim == null)
            throw new UnauthorizedAccessException("No se pudo obtener el id de usuario del token.");
        return int.Parse(claim.Value);
    }

    // ---------------------------------------------------------
    //  POSTS
    // ---------------------------------------------------------

    /// <summary>
    /// Da "Me gusta" a un post
    /// </summary>
    [HttpPost("posts/{postId}/like")]
    public async Task<ActionResult<PostLikesDto>> LikePost(int postId)
    {
        try
        {
            var usuarioId = GetUsuarioIdFromToken();

            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                return NotFound(new { Message = "El post no existe." });

            var result = await _likeService.LikePostAsync(postId, usuarioId);

            // Notificación (opcional)
            if (post.UsuarioId != usuarioId)
            {
                await _notificacionService.CrearNotificacionLikePostAsync(
                    post.UsuarioId,
                    usuarioId,
                    postId
                );

                await _notificacionesHub
                    .Clients.User(post.UsuarioId.ToString())
                    .SendAsync(
                        "NotificacionRecibida",
                        new
                        {
                            Tipo = "LikePost",
                            PostId = postId,
                            DeUsuarioId = usuarioId,
                        }
                    );
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new { Message = "Error al dar like al post.", Error = ex.Message }
            );
        }
    }

    /// <summary>
    /// Quita "Me gusta" a un post
    /// </summary>
    [HttpPost("posts/{postId}/unlike")]
    public async Task<ActionResult<PostLikesDto>> UnlikePost(int postId)
    {
        try
        {
            var usuarioId = GetUsuarioIdFromToken();

            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                return NotFound(new { Message = "El post no existe." });

            var result = await _likeService.UnlikePostAsync(postId, usuarioId);

            // Opcional: notificación inversa o nada
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new { Message = "Error al quitar like al post.", Error = ex.Message }
            );
        }
    }

    // ---------------------------------------------------------
    //  COMENTARIOS
    // ---------------------------------------------------------

    /// <summary>
    /// Da "Me gusta" a un comentario
    /// </summary>
    [HttpPost("comentarios/{comentarioId}/like")]
    public async Task<ActionResult<ComentarioLikesDto>> LikeComentario(int comentarioId)
    {
        try
        {
            var usuarioId = GetUsuarioIdFromToken();

            var comentario = await _comentarioRepository.GetByIdAsync(comentarioId);
            if (comentario == null)
                return NotFound(new { Message = "El comentario no existe." });

            var result = await _likeService.LikeComentarioAsync(comentarioId, usuarioId);

            // Notificación (opcional)
            if (comentario.UsuarioId != usuarioId)
            {
                await _notificacionService.CrearNotificacionLikeComentarioAsync(
                    comentario.UsuarioId.Value,
                    usuarioId,
                    comentarioId
                );

                await _notificacionesHub
                    .Clients.User(comentario.UsuarioId.ToString())
                    .SendAsync(
                        "NotificacionRecibida",
                        new
                        {
                            Tipo = "LikeComentario",
                            ComentarioId = comentarioId,
                            DeUsuarioId = usuarioId,
                        }
                    );
            }

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new { Message = "Error al dar like al comentario.", Error = ex.Message }
            );
        }
    }

    /// <summary>
    /// Quita "Me gusta" a un comentario
    /// </summary>
    [HttpPost("comentarios/{comentarioId}/unlike")]
    public async Task<ActionResult<ComentarioLikesDto>> UnlikeComentario(int comentarioId)
    {
        try
        {
            var usuarioId = GetUsuarioIdFromToken();

            var comentario = await _comentarioRepository.GetByIdAsync(comentarioId);
            if (comentario == null)
                return NotFound(new { Message = "El comentario no existe." });

            var result = await _likeService.UnlikeComentarioAsync(comentarioId, usuarioId);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new { Message = "Error al quitar like al comentario.", Error = ex.Message }
            );
        }
    }
}
