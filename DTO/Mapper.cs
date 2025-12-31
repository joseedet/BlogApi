using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Models;

namespace BlogApi.DTO;

public static class Mapper
{
    public static CategoriaDto ToDto(this Categoria categoria) =>
        new CategoriaDto { Id = categoria.Id, Nombre = categoria.Nombre };

    public static UsuarioDto ToDto(this Usuario usuario) =>
        new UsuarioDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Rol = usuario.Rol,
        };

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

    public static PostDto ToDto(this Post post) =>
        new PostDto
        {
            Id = post.Id,
            Titulo = post.Titulo,
            Contenido = post.Contenido,
            FechaCreacion = post.FechaCreacion,
            FechaActualizacion = post.FechaActualizacion,

            Categoria = post.Categoria.ToDto(),
            Usuario = post.Usuario.ToDto(),

            Comentarios = post.Comentarios.Select(c => c.ToDto()).ToList(),
            Tags = post.Tags.Select(t => t.ToDto()).ToList(),
        };
    public static TagDto ToDto(this Tag tag) => new TagDto { Id = tag.Id, Nombre = tag.Nombre };
}
