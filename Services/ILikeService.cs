using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.DTO;

namespace BlogApi.Services;

/// <summary>
///   Servicio para manejar los "Me gusta" en posts y comentarios
/// </summary>
public interface ILikeService
{
    /// <summary>
    ///  Da "Me gusta" a un post por parte de un usuario
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="usuarioId"></param>
    /// <returns>PostLikesDto</returns>
    /// </summary>
    Task<PostLikesDto> LikePostAsync(int postId, int usuarioId);

    /// <summary>
    /// Quita el "Me gusta" a un post por parte de un usuario
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="usuarioId"></param>
    /// <returns>PostLikesDto</returns>
    /// </summary>
    Task<PostLikesDto> UnlikePostAsync(int postId, int usuarioId);

    /// <summary>
    ///  Da "Me gusta" a un comentario por parte de un usuario
    /// </summary>
    /// <param name="comentarioId"></param>
    /// <param name="usuarioId"></param>
    /// <returns>ComentarioLikesDto</returns>
    /// </summary>
    Task<ComentarioLikesDto> LikeComentarioAsync(int comentarioId, int usuarioId);

    /// <summary>
    /// Quita el "Me gusta" a un comentario por parte de un usuario
    /// </summary>
    /// <param name="comentarioId"></param>
    /// <param name="usuarioId"></param>
    /// <returns>ComentarioLikesDto</returns>
    /// </summary>
    Task<ComentarioLikesDto> UnlikeComentarioAsync(int comentarioId, int usuarioId);
}
