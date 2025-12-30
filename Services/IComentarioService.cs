using BlogApi.Models;

namespace BlogApi.Services;

public interface IComentarioService
{
    Task<IEnumerable<Comentario>> GetComentariosDePostAsync(int postId);
    Task<Comentario> CrearComentarioAsync(Comentario comentario);
    Task<bool> CambiarEstadoAsync(int id, string estado);

    Task<bool> EliminarComentarioAsync(int comentarioId, int usuarioId, bool puedeBorrarTodo);
}
