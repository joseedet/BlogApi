using System.Text.Json;
using BlogApi.DTO;
using BlogApi.Models;

namespace BlogApi.Mapper;

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
            //Rol = usuario.Rol,
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
            FechaCreacion = comentario.FechaCreacion,
            Estado = comentario.Estado.ToString(),
            Usuario = comentario.Usuario != null ? comentario.Usuario.ToDto() : null,
            Respuestas = comentario.Respuestas?.Select(r => r.ToDto()).ToList() ?? new(),
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
            Categoria = post.Categoria != null ? post.Categoria.ToDto() : null,
            Usuario = post.Usuario != null ? post.Usuario.ToDto() : null,
            Comentarios = post.Comentarios.Select(c => c.ToDto()).ToList(),
            Tags = post.Tags.Select(t => t.ToDto()).ToList(),
            ViewsCount = post.ViewsCount,
        };

    /// <summary>
    ///  Convierte una entidad Tag a su DTO correspondiente.
    ///</summary>
    /// <param name="tag"></param>
    /// <returns>TagDto</returns>
    public static TagDto ToDto(this Tag tag) => new() { Id = tag.Id, Nombre = tag.Nombre };

    /// <summary>
    ///  Convierte una entidad Notificación a su DTO correspondiente.
    /// </summary>
    /// <param name="n"></param>
    /// <returns>NotificacionDto</returns>
    public static NotificacionDto ToDto(this Notificacion n)
    {
        var dto = new NotificacionDto
        {
            Id = n.Id,
            UsuarioDestinoId = n.UsuarioDestinoId,
            UsuarioOrigenId = n.UsuarioOrigenId,
            Tipo = n.Tipo,
            Mensaje = n.Mensaje,
            FechaCreacion = n.FechaCreacion,
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

    /// <summary>
    /// Entidad NotificationSettings a NotificationSettingsDto
    /// </summary>
    /// <param name="settings"></param>
    /// <returns>NotificationSettingsDto</returns>
    public static NotificationSettingsDto ToDto(NotificationSettings settings)
    {
        return new NotificationSettingsDto
        {
            SendEmailOnComment = settings.SendEmailOnComment,
            SendEmailOnAdminMessage = settings.SendEmailOnAdminMessage,
            SendEmailOnSystemAlert = settings.SendEmailOnSystemAlert,
        };
    }

    /// <summary>
    /// Actualización de NotificationSettingsDto a NotificationSettings
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="dto"></param>
    public static void UpdateFromDto(NotificationSettings entity, NotificationSettingsDto dto)
    {
        entity.SendEmailOnComment = dto.SendEmailOnComment;
        entity.SendEmailOnAdminMessage = dto.SendEmailOnAdminMessage;
        entity.SendEmailOnSystemAlert = dto.SendEmailOnSystemAlert;
    }

    /// <summary>
    /// Convierte una lista de entidades Post a una lista de PostDto.
    /// </summary>
    /// <returns>List&lt;PostDto&gt;</returns>
    public static List<PostDto> ToDto(this List<Post> posts)
    {
        return posts.Select(p => p.ToDto()).ToList();
    }
    public static PageDto ToDto(this Page page) =>
        new PageDto
        {
            Id = page.Id,
            Titulo = page.Titulo,
            Contenido = page.Contenido,
            Slug = page.Slug,
            Publicado = page.Publicado,
            EsInicio = page.EsInicio,

            // SEO
            MetaTitulo = page.MetaTitulo,
            MetaDescripcion = page.MetaDescripcion,
            MetaKeywords = page.MetaKeywords,

            // Auditoría
            Creado = page.Creado,
            Actualizado = page.Actualizado,
            IpCreacion = page.IpCreacion,
            UserAgentCreacion = page.UserAgentCreacion,
            IpActualizacion = page.IpActualizacion,
            UserAgentActualizacion = page.UserAgentActualizacion,
        };
}
