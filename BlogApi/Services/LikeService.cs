using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services.Interfaces;

namespace BlogApi.Services;

/// <summary>
///         Servicio para manejar los "Me gusta" en posts y comentarios
/// </summary>
public class LikeService : ILikeService
{
    /// <summary>
    /// Repositorio para los "Me gusta" en posts
    /// </summary>
    private readonly ILikePostRepository _likePostRepo;

    /// <summary>
    /// Repositorio para los "Me gusta" en comentarios
    /// </summary>
    private readonly ILikeComentarioRepository _likeComentarioRepo;

    /// <summary>
    /// Constructor del servicio
    /// </summary>
    public LikeService(
        ILikePostRepository likePostRepo,
        ILikeComentarioRepository likeComentarioRepo
    )
    { // Inicializa los repositorios
        _likePostRepo = likePostRepo;
        _likeComentarioRepo = likeComentarioRepo;
    }

    // ---------------------------------------------------------
    //  POSTS
    // ---------------------------------------------------------
    /// <summary>
    /// Da "Me gusta" a un post por parte de un usuario
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="usuarioId"></param>
    /// <returns>PostLikesDto</returns>
    /// </summary>
    public async Task<PostLikesDto> LikePostAsync(int postId, int usuarioId)
    {
        var existe = await _likePostRepo.ExisteAsync(postId, usuarioId);

        if (!existe)
        {
            await _likePostRepo.CrearAsync(new LikePost { PostId = postId, UsuarioId = usuarioId });
        }

        return new PostLikesDto
        {
            PostId = postId,
            UsuarioId = usuarioId,
            UsuarioHaDadoLike = true,
            TotalLikes = await _likePostRepo.ContarAsync(postId),
        };
    }

    /// <summary>
    /// Quita el "Me gusta" a un post por parte de un usuario
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="usuarioId"></param>
    /// <returns>PostLikesDto</returns>
    /// </summary>
    public async Task<PostLikesDto> UnlikePostAsync(int postId, int usuarioId)
    {
        var existe = await _likePostRepo.ExisteAsync(postId, usuarioId);

        if (existe)
            await _likePostRepo.EliminarAsync(postId, usuarioId);

        return new PostLikesDto
        {
            PostId = postId,
            UsuarioId = usuarioId,
            UsuarioHaDadoLike = false,
            TotalLikes = await _likePostRepo.ContarAsync(postId),
        };
    }

    // ---------------------------------------------------------
    //  COMENTARIOS
    // ---------------------------------------------------------
    /// <summary>
    ///  Da "Me gusta" a un comentario por parte de un usuario
    /// </summary>
    /// <param name="comentarioId"></param>
    /// <param name="usuarioId"></param>
    /// <returns>ComentarioLikesDto</returns>
    /// </summary>
    public async Task<ComentarioLikesDto> LikeComentarioAsync(int comentarioId, int usuarioId)
    {
        var existe = await _likeComentarioRepo.ExisteAsync(comentarioId, usuarioId);

        if (!existe)
        {
            await _likeComentarioRepo.CrearAsync(
                new LikeComentario { ComentarioId = comentarioId, UsuarioId = usuarioId }
            );
        }

        return new ComentarioLikesDto
        {
            ComentarioId = comentarioId,
            UsuarioId = usuarioId,
            UsuarioHaDadoLike = true,
            TotalLikes = await _likeComentarioRepo.ContarAsync(comentarioId),
        };
    }

    /// <summary>
    /// Quita el "Me gusta" a un comentario por parte de un usuario
    /// </summary>
    /// <param name="comentarioId"></param>
    /// <param name="usuarioId"></param>
    /// <returns>ComentarioLikesDto</returns>
    /// </summary>
    public async Task<ComentarioLikesDto> UnlikeComentarioAsync(int comentarioId, int usuarioId)
    {
        var existe = await _likeComentarioRepo.ExisteAsync(comentarioId, usuarioId);

        if (existe)
            await _likeComentarioRepo.EliminarAsync(comentarioId, usuarioId);

        return new ComentarioLikesDto
        {
            ComentarioId = comentarioId,
            UsuarioId = usuarioId,
            UsuarioHaDadoLike = false,
            TotalLikes = await _likeComentarioRepo.ContarAsync(comentarioId),
        };
    }
}
