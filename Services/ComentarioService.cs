using BlogApi.Models;
using BlogApi.Repositories;

namespace BlogApi.Services;

public class ComentarioService : IComentarioService
{
    private readonly IComentarioRepository _repo;

    public ComentarioService(IComentarioRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Comentario>> GetComentariosDePostAsync(int postId) =>
        await _repo.GetByPostIdAsync(postId);

    public async Task<Comentario> CrearComentarioAsync(Comentario comentario)
    {
        comentario.Fecha = DateTime.UtcNow;
        comentario.Estado = "pendiente";
        await _repo.AddAsync(comentario);
        await _repo.SaveChangesAsync();
        return comentario;
    }

    public async Task<bool> CambiarEstadoAsync(int id, string estado)
    {
        var comentario = await _repo.GetByIdAsync(id);
        if (comentario == null)
            return false;
        comentario.Estado = estado;
        _repo.Update(comentario);
        await _repo.SaveChangesAsync();
        return true;
    }
    public async Task<bool> EliminarComentarioAsync(
        int comentarioId,
        int usuarioId,
        bool puedeBorrarTodo
    )
    {
        var comentario = await _repo.GetByIdAsync(comentarioId);
        if (comentario == null)
            return false;

        // Si no es admin/editor, solo puede borrar su propio comentario
        if (!puedeBorrarTodo && comentario.UsuarioId != usuarioId)
            return false;

        _repo.Remove(comentario);
        await _repo.SaveChangesAsync();
        return true;
    }
}
