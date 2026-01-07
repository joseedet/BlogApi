using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Repositories;

namespace BlogApi.Services;

/// <summary>
///   Servicio para manejar los "Me gusta" en posts y comentarios
/// </summary>
public class LikeService : ILikeService
{
    /// <summary>
    ///   Repositorios necesarios para manejar los "Me gusta"
    /// </summary>
    private readonly ILikePostRepository _likePostRepo;

    /// <summary>
    ///   Repositorios necesarios para manejar los "Me gusta" en comentarios
    /// </summary>
    private readonly ILikeComentarioRepository _likeComentarioRepo;

    /// <summary>
    ///   Constructor del servicio de "Me gusta"
    /// </summary>
    /// <param name="likePostRepo"></param>
    /// <param name="likeComentarioRepo"></param>
    /// <returns></returns>
    /// <summary>
    public LikeService(
        ILikePostRepository likePostRepo,
        ILikeComentarioRepository likeComentarioRepo
    )
    {
        _likePostRepo = likePostRepo;
        _likeComentarioRepo = likeComentarioRepo;
    }

    /// <summary>
    ///   Da "Me gusta" a un post por parte de un usuario
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="usuarioId"></param>
    /// <returns>PostLikesDto</returns>
    /// </summary>
    public async Task<PostLikesDto> LikePostAsync(int postId, int usuarioId)
    {
        if (!await _likePostRepo.ExisteAsync(postId, usuarioId))
            await _likePostRepo.CrearAsync(new LikePost { PostId = postId, UsuarioId = usuarioId });
        return new PostLikesDto
        {
            PostId = postId,
            TotalLikes = await _likePostRepo.ContarAsync(postId),
            UsuarioHaDadoLike = true,
        };
    }

    /// <summary>
    ///   Quita el "Me gusta" a un post por parte de un usuario
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="usuarioId"></param>
    /// <returns>PostLikesDto</returns>
    /// </summary>
    public async Task<PostLikesDto> UnlikePostAsync(int postId, int usuarioId)
    {
        await _likePostRepo.EliminarAsync(postId, usuarioId);
        return new PostLikesDto
        {
            PostId = postId,
            TotalLikes = await _likePostRepo.ContarAsync(postId),
            UsuarioHaDadoLike = false,
        };
    }

    /// <summary>
    ///   Da "Me gusta" a un comentario por parte de un usuario
    /// </summary>
    /// <param name="comentarioId"></param>
    /// <param name="usuarioId"></param>
    /// <returns>ComentarioLikesDto</returns>
    /// </summary>
    public async Task<ComentarioLikesDto> LikeComentarioAsync(int comentarioId, int usuarioId)
    {
        if (!await _likeComentarioRepo.ExisteAsync(comentarioId, usuarioId))
            await _likeComentarioRepo.CrearAsync(
                new LikeComentario { ComentarioId = comentarioId, UsuarioId = usuarioId }
            );
        return new ComentarioLikesDto
        {
            ComentarioId = comentarioId,
            TotalLikes = await _likeComentarioRepo.ContarAsync(comentarioId),
            UsuarioHaDadoLike = true,
        };
    }

    /// <summary>
    ///   Quita el "Me gusta" a un comentario por parte de un usuario
    /// </summary>
    /// <param name="comentarioId"></param>
    /// <param name="usuarioId"></param>
    /// <returns>ComentarioLikesDto</returns>
    /// </summary>
    public async Task<ComentarioLikesDto> UnlikeComentarioAsync(int comentarioId, int usuarioId)
    {
        await _likeComentarioRepo.EliminarAsync(comentarioId, usuarioId);
        return new ComentarioLikesDto
        {
            ComentarioId = comentarioId,
            TotalLikes = await _likeComentarioRepo.ContarAsync(comentarioId),
            UsuarioHaDadoLike = false,
        };
    }
}
