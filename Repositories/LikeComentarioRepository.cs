using BlogApi.Data;
using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repositories;

/// <summary>
///   Repositorio para gestionar "Me gusta" en comentarios
/// </summary>
public class LikeComentarioRepository : ILikeComentarioRepository
{
    /// <summary>
    ///   Contexto de la base de datos
    /// </summary>
    private readonly BlogDbContext _db;

    /// <summary>
    ///  Constructor del repositorio
    /// </summary>
    /// <param name="db"></param>
    public LikeComentarioRepository(BlogDbContext db)
    {
        _db = db;
    }

    /// <summary>
    ///   Verifica si un usuario ha dado "Me gusta" a un comentario
    /// </summary>
    /// <param name="comentarioId"></param>
    /// <param name="usuarioId"></param>
    /// <returns>true si existe, false si no</returns>
    /// <summary>
    public Task<bool> ExisteAsync(int comentarioId, int usuarioId) =>
        _db.LikesComentario.AnyAsync(x =>
            x.ComentarioId == comentarioId && x.UsuarioId == usuarioId
        );

    /// <summary>
    ///   Cuenta el total de "Me gusta" en un comentario
    /// </summary>
    /// <param name="comentarioId"></param>
    /// <returns>Total de "Me gusta"</returns>
    /// <summary>
    public Task<int> ContarAsync(int comentarioId) =>
        _db.LikesComentario.CountAsync(x => x.ComentarioId == comentarioId);

    /// <summary>
    ///   Crea un nuevo "Me gusta" en un comentario
    /// </summary>
    /// <param name="like"></param>
    /// <returns></returns>
    /// <summary>
    public async Task CrearAsync(LikeComentario like)
    {
        _db.LikesComentario.Add(like);
        await _db.SaveChangesAsync();
    }

    /// <summary>
    ///   Elimina un "Me gusta" de un comentario
    /// </summary>
    /// <param name="comentarioId"></param>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    /// <summary>
    public async Task EliminarAsync(int comentarioId, int usuarioId)
    {
        var like = await _db.LikesComentario.FirstOrDefaultAsync(x =>
            x.ComentarioId == comentarioId && x.UsuarioId == usuarioId
        );

        if (like != null)
        {
            _db.LikesComentario.Remove(like);
            await _db.SaveChangesAsync();
        }
    }
}
