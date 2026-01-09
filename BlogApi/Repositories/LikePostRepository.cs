using BlogApi.Data;
using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repositories;

/// <summary>
///   Repositorio para manejar los "Me gusta" en publicaciones
/// </summary>
public class LikePostRepository : ILikePostRepository
{
    /// <summary>
    ///   Contexto de la base de datos
    /// </summary>
    private readonly BlogDbContext _db;

    /// <summary>
    ///  Constructor del repositorio
    /// </summary>
    /// <param name="db"></param>
    /// <summary>
    public LikePostRepository(BlogDbContext db)
    {
        _db = db;
    }

    /// <summary>
    ///   Verifica si un usuario ha dado "Me gusta" a un post
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="usuarioId"></param>
    /// <returns>true si existe, false si no</returns>
    /// <summary>
    public Task<bool> ExisteAsync(int postId, int usuarioId) =>
        _db.LikesPost.AnyAsync(x => x.PostId == postId && x.UsuarioId == usuarioId);

    /// <summary>
    ///   Cuenta el total de "Me gusta" en un post
    /// </summary>
    /// <param name="postId"></param>
    /// <returns>Total de "Me gusta"</returns>
    /// <summary>
    public Task<int> ContarAsync(int postId) => _db.LikesPost.CountAsync(x => x.PostId == postId);

    /// <summary>
    ///   Elimina un "Me gusta" de un post
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    /// <summary>
    public async Task EliminarAsync(int postId, int usuarioId)
    {
        var like = await _db.LikesPost.FirstOrDefaultAsync(x =>
            x.PostId == postId && x.UsuarioId == usuarioId
        );
        if (like != null)
        {
            _db.LikesPost.Remove(like);
            await _db.SaveChangesAsync();
        }
    }

    /// <summary>
    ///   Crea un nuevo "Me gusta" en un post
    /// </summary>
    /// <param name="like"></param>
    /// <returns></returns>
    /// <summary>
    public async Task CrearAsync(LikePost like)
    {
        _db.LikesPost.Add(like);
        await _db.SaveChangesAsync();
    }
}
