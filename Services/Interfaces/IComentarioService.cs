using BlogApi.Models;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Servicio para gestionar comentarios.
/// </summary>
public interface IComentarioService
{
    /// <summary>
    ///   Obtiene los comentarios raíz de un post específico.
    /// </summary>
    /// <param name="postId"></param>
    /// <returns>Comentarios raíz del post</returns>
    /// </summary>
    Task<IEnumerable<Comentario>> GetComentariosDePostAsync(int postId);

    /// <summary>
    /// Crea un nuevo comentario o respuesta.
    /// </summary>
    /// <param name="comentario"></param>
    /// <returns>El comentario creado</returns>
    Task<Comentario> CrearComentarioAsync(Comentario comentario);

    /// <summary>
    /// Cambia el estado de un comentario.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="estado"></param>
    /// <returns>True si se cambió el estado, false si no se encontró el comentario</returns>
    /// </summary>
    Task<bool> CambiarEstadoAsync(int id, string estado);

    /// <summary>
    /// Obtiene comentarios por estado.
    /// </summary>
    /// <param name="estado"></param>
    /// <returns>Lista de comentarios con el estado especificado</returns>
    /// </summary>
    Task<IEnumerable<Comentario>> GetByEstadoAsync(string estado);

    /// <summary>
    /// Obtiene un comentario por su ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>El comentario si se encuentra, null si no</returns>
    /// </summary>
    Task<Comentario?> GetByIdAsync(int id);

    /// <summary>
    /// Elimina un comentario por su ID.
    /// </summary>
    /// <param name="comentarioId"></param>
    /// <param name="usuarioId"></param>
    /// <param name="puedeBorrarTodo"></param>
    /// <returns>True si se eliminó el comentario, false si no se encontró o no se pudo eliminar</returns>
    /// </summary>
    Task<bool> EliminarComentarioAsync(int comentarioId, int usuarioId, bool puedeBorrarTodo);

   
}
