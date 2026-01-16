using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BlogApi.Models;

namespace BlogApi.DTO;

/// <summary>
/// Clase estática que contiene métodos de mapeo entre entidades y DTOs.
/// </summary>
public static class Mapper
{
    /// <summary>
    ///     Convierte una entidad Categoria a su DTO correspondiente.
    /// </summary>
    /// <param name="categoria"></param>
    /// <returns>CategoriaDto </returns>
    /// </summary>
    public static CategoriaDto ToDto(this Categoria categoria)
    {
        return new() { Id = categoria.Id, Nombre = categoria.Nombre };
    }

    /// <summary>
    ///    Convierte una entidad Usuario a su DTO correspondiente.
    /// </summary>
    /// <param name="usuario"></param>
    /// <returns>UsuarioDto</returns>
    /// </summary>
    public static UsuarioDto ToDto(this Usuario usuario) =>
        new UsuarioDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Rol = usuario.Rol,
        };

    /// <summary>
    ///   Convierte una entidad Comentario a su DTO correspondiente.
    /// </summary>
    /// <param name="comentario"></param>
    /// <returns>ComentarioDto</returns>
    /// </summary>
    public static ComentarioDto ToDto(this Comentario comentario) =>
        new ComentarioDto
        {
            Id = comentario.Id,
            Contenido = comentario.Contenido,
            Fecha = comentario.Fecha,
            Estado = comentario.Estado,
            Usuario = comentario.Usuario?.ToDto(),
            Respuestas = comentario.Respuestas.Select(r => r.ToDto()).ToList(),
        };

    /// <summary>
    ///   Convierte una entidad Post a su DTO correspondiente.
    /// </summary>
    /// <param name="post"></param>
    /// <returns>PostDto</returns>
    /// </summary>
    public static PostDto ToDto(this Post post) =>
        new PostDto
        {
            Id = post.Id,
            Titulo = post.Titulo,
            Contenido = post.Contenido,
            Slug = post.Slug,
            FechaCreacion = post.FechaCreacion,
            FechaActualizacion = post.FechaActualizacion,

            Categoria = post.Categoria.ToDto(),
            Usuario = post.Usuario.ToDto(),

            Comentarios = post.Comentarios.Select(c => c.ToDto()).ToList(),
            Tags = post.Tags.Select(t => t.ToDto()).ToList(),
        };

    /// <summary>
    ///  Convierte una entidad Tag a su DTO correspondiente.
    ///</summary>
    /// <param name="tag"></param>
    /// <returns>TagDto</returns>
    /// </summary>
    public static TagDto ToDto(this Tag tag) => new() { Id = tag.Id, Nombre = tag.Nombre };


    /// <summary>
    ///  Convierte una entidad Notificación a su DTO correspondiente.
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static NotificacionDto ToDto(this Notificacion n)
    {
        var dto = new NotificacionDto
        {
            Id = n.Id,
            UsuarioDestinoId = n.UsuarioId,
            UsuarioOrigenId = 0, // si luego quieres añadirlo, aquí se ajusta
            Tipo = n.Tipo,
            Mensaje = n.Mensaje,
            Fecha = n.Fecha,
            Leida = n.Leida,
            Payload = n.Payload,
        };

        // Extraer datos del payload si existe
        if (!string.IsNullOrWhiteSpace(n.Payload))
        {
            try
            {
                var data = JsonSerializer.Deserialize<Dictionary<string, int>>(n.Payload);

                if (data != null)
                {
                    if (data.TryGetValue("postId", out var postId))
                        dto.PostId = postId;

                    if (data.TryGetValue("comentarioId", out var comentarioId))
                        dto.ComentarioId = comentarioId;
                }
            }
            catch
            {
                // Si el payload no es válido, simplemente lo ignoramos
            }
        }

        return dto;
    }
}
