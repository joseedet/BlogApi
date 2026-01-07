using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Models;

namespace BlogApi.Repositories;

/// <summary>
/// Interfaz para el repositorio de "Me gusta" en comentarios
/// </summary>
public interface ILikeComentarioRepository
{
    /// <summary>
    /// Verifica si un usuario ha dado "Me gusta" a un comentario
    /// </summary>
    /// <param name="comentarioId"></param>
    /// <param name="usuarioId"></param>
    /// <returns>true si existe, false si no</returns>
    /// <summary>
    Task<bool> ExisteAsync(int comentarioId, int usuarioId);

    /// <summary>
    /// Cuenta el total de "Me gusta" en un comentario
    /// </summary>
    /// <param name="comentarioId"></param>
    /// <returns>Total de "Me gusta"</returns>
    /// <summary>
    Task<int> ContarAsync(int comentarioId);

    /// <summary>
    /// Crea un nuevo "Me gusta" en un comentario
    /// </summary>
    /// <param name="like"></param>
    /// <returns></returns>
    /// <summary>
    Task CrearAsync(LikeComentario like);

    /// <summary>
    /// Elimina un "Me gusta" de un comentario
    /// </summary>
    /// <param name="comentarioId"></param>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    /// <summary>
    Task EliminarAsync(int comentarioId, int usuarioId);
}
