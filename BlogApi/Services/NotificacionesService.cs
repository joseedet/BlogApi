using BlogApi.Data;
using BlogApi.DTO;
using BlogApi.Hubs;
using BlogApi.Mapper;
using BlogApi.Models;
using BlogApi.Services.Interfaces;
using BlogApi.Utils;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BlogApi.Services;

/// <summary>
/// Clase NotificacionesService
/// </summary>
public class NotificacionesService : INotificacionesService
{
    private readonly BlogDbContext _db;
    private readonly IHubContext<NotificacionesHub> _hub;
    private readonly IEmailService _email;
    private readonly ILogger<NotificacionesService> _logger;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly IOptions<AppSettings> _settings;

    /// <summary>
    /// Constructor NotificacionesService
    /// </summary>
    /// <param name="db"></param>
    /// <param name="hub"></param>
    /// <param name="email"></param>
    /// <param name="emailTemplateService"></param>
    /// <param name="logger"></param>
    /// <param name="settings"></param>
    public NotificacionesService(
        BlogDbContext db,
        IHubContext<NotificacionesHub> hub,
        IEmailService email,
        ILogger<NotificacionesService> logger,
        IEmailTemplateService emailTemplateService,
        IOptions<AppSettings> settings
    )
    {
        _db = db;
        _hub = hub;
        _email = email;
        _logger = logger;
        _emailTemplateService = emailTemplateService;
        _settings = settings;
    }

    // ------------------------------------------------------------
    // Crear notificación
    // ------------------------------------------------------------

    /// <summary>
    /// Crea notificación
    /// </summary>
    /// <param name="notificacion"></param>
    /// <returns></returns>
    public async Task CrearAsync(Notificacion notificacion)
    {
        _db.Notificaciones.Add(notificacion);
        await _db.SaveChangesAsync();

        try
        {
            // 1. SignalR
            await _hub
                .Clients.User(notificacion.UsuarioDestinoId.ToString())
                .SendAsync("NuevaNotificacion", notificacion.ToDto());

            _logger.LogInformation(
                "Notificación enviada por SignalR al usuario {UserId}",
                notificacion.UsuarioDestinoId
            );

            // 2. Email (solo si aplica)
            await EnviarEmailNotificacion(notificacion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando notificación");
        }
    }

    private async Task EnviarEmailNotificacion(Notificacion notificacion)
    {
        // 1. Obtener datos necesarios
        var usuario = await _db.Usuarios.FindAsync(notificacion.UsuarioDestinoId);
        var post = await _db.Posts.FindAsync(notificacion.PostId);

        if (usuario == null || post == null)
            return;

        // 2. Construir modelo del correo
        var model = new CommentNotificationEmailModel
        {
            UserName = usuario.Nombre,
            PostTitle = post.Titulo,
            Email = usuario.Email,
        };

        // 3. Variables para la plantilla
        var variables = new Dictionary<string, string>
        {
            { "USER_NAME", model.UserName },
            { "POST_TITLE", model.PostTitle },
            { "APP_NAME", _settings.Value.AppName },
            { "SUBJECT", "Nuevo comentario en tu post" },
        };

        // 4. Renderizar plantilla completa
        var html = await _emailTemplateService.RenderTemplateAsync(
            "Notifications/comment-notification.html",
            variables
        );

        // 5. Enviar correo
        await _email.EnviarAsync(model.Email, "Nuevo comentario en tu post", html);
    }

    // ------------------------------------------------------------
    // Crear notificación de Like en Post
    // ------------------------------------------------------------

    /// <summary>
    /// Crea notificacion cuando hay un Like en un post
    /// </summary>
    /// <param name="usuarioDestinoId"></param>
    /// <param name="usuarioOrigenId"></param>
    /// <param name="postId"></param>
    /// <returns></returns>
    public async Task CrearNotificacionLikePostAsync(
        int usuarioDestinoId,
        int usuarioOrigenId,
        int postId
    )
    {
        var notificacion = new Notificacion
        {
            UsuarioDestinoId = usuarioDestinoId,
            UsuarioOrigenId = usuarioOrigenId,
            Tipo = TipoNotificacion.LikePost,
            PostId = postId,
            Mensaje = $"Al usuario {usuarioOrigenId} le gustó tu post.",
            FechaCreacion = DateTime.UtcNow,
            Leida = false,
            Payload = $"{{ \"postId\": {postId}, \"usuarioOrigenId\": {usuarioOrigenId} }}",
        };

        await CrearAsync(notificacion);
    }

    // ------------------------------------------------------------
    // Crear notificación de Like en Comentario
    // ------------------------------------------------------------

    /// <summary>
    /// Crea notificacion cuando hay un like en un comentario
    /// </summary>
    /// <param name="usuarioDestinoId"></param>
    /// <param name="usuarioOrigenId"></param>
    /// <param name="comentarioId"></param>
    /// <returns></returns>
    public async Task CrearNotificacionLikeComentarioAsync(
        int usuarioDestinoId,
        int usuarioOrigenId,
        int comentarioId
    )
    {
        // Obtener el comentario para extraer el PostId
        var comentario = await _db
            .Comentarios.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == comentarioId);
        if (comentario == null)
            return; // o lanzar excepción si prefieres
        var postId = comentario.PostId;
        var notificacion = new Notificacion
        {
            UsuarioDestinoId = usuarioDestinoId,
            UsuarioOrigenId = usuarioOrigenId,
            Tipo = TipoNotificacion.LikeComentario,
            ComentarioId = comentarioId,
            Mensaje = $"Al usuario {usuarioOrigenId} le gustó tu comentario.",
            FechaCreacion = DateTime.UtcNow,
            Leida = false,
            Payload = $"{{ \"comentarioId\": {comentarioId}, \"postId\": {postId} }}",
        };

        await CrearAsync(notificacion);
    }

    // ------------------------------------------------------------
    // Obtener todas las notificaciones del usuario
    // ------------------------------------------------------------

    /// <summary>
    /// obtiene notificaciones por usuario
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns>IEnumerable de NotificacionDto</returns>
    public async Task<IEnumerable<NotificacionDto>> ObtenerPorUsuarioAsync(int usuarioId)
    {
        return await _db
            .Notificaciones.Where(n => n.UsuarioDestinoId == usuarioId)
            .OrderByDescending(n => n.FechaCreacion)
            .Select(n =>
                n.ToDto() /*ToDto(n)*/
            )
            .ToListAsync();
    }

    // ------------------------------------------------------------
    // Obtener una notificación por ID
    // ------------------------------------------------------------

    /// <summary>
    /// Obtiene notificación
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Notificación o un valor null</returns>
    public async Task<Notificacion?> GetByIdAsync(int id)
    {
        return await _db.Notificaciones.FirstOrDefaultAsync(n => n.Id == id);
    }

    // ------------------------------------------------------------
    // Marcar una notificación como leída
    // ------------------------------------------------------------

    /// <summary>
    /// Marcar como leído
    /// </summary>
    /// <param name="id"></param>
    /// <param name="usuarioId"></param>
    /// <returns>Devuelve verdadero si ha sido marcada como leida en caso contrario falso</returns>
    public async Task<bool> MarcarComoLeidaAsync(int id, int usuarioId)
    {
        var notif = await _db.Notificaciones.FirstOrDefaultAsync(n => n.Id == id);

        if (notif == null || notif.UsuarioDestinoId != usuarioId)
            return false;

        notif.Leida = true;
        await _db.SaveChangesAsync();
        return true;
    }

    // ------------------------------------------------------------
    // Marcar todas como leídas
    // ------------------------------------------------------------

    /// <summary>
    /// Marca todas las notificaciones como leidas.
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    public async Task MarcarTodasComoLeidasAsync(int usuarioId)
    {
        var notis = await _db
            .Notificaciones.Where(n => n.UsuarioDestinoId == usuarioId && !n.Leida)
            .ToListAsync();

        foreach (var n in notis)
            n.Leida = true;

        await _db.SaveChangesAsync();
    }

    // ------------------------------------------------------------
    // Eliminar notificación
    // ------------------------------------------------------------

    /// <summary>
    /// Elimina notificacion
    /// </summary>
    /// <param name="id"></param>
    /// <param name="usuarioId"></param>
    /// <returns>Verdadero si ha sido eliminada, en caso contrario falso </returns>
    public async Task<bool> EliminarAsync(int id, int usuarioId)
    {
        var notif = await GetByIdAsync(id);

        if (notif == null || notif.UsuarioDestinoId != usuarioId)
            return false;

        _db.Notificaciones.Remove(notif);
        await _db.SaveChangesAsync();
        return true;
    }

    /*     // ------------------------------------------------------------
        // Obtener no leídas
        // ------------------------------------------------------------
    
        /// <summary>
        /// Obtiene notificaciones no leidas
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <returns>Lista de NotificacionesDto</returns>
        public async Task<List<NotificacionDto>> ObtenerNoLeidasAsync(int usuarioId)
        {
            return await _db
                .Notificaciones.Where(n => n.UsuarioDestinoId == usuarioId && !n.Leida)
                .OrderByDescending(n => n.Fecha)
                .Select(n => ToDto(n))
                .ToListAsync();
        }
     */

    // ------------------------------------------------------------
    // Obtener paginadas
    // ------------------------------------------------------------

    /// <summary>
    /// Pagina los resultados de NotificacionDto
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns>PaginacionResultado NotificacionDto</returns>
    public async Task<PaginacionResultado<NotificacionDto>> GetPaginadasAsync(
        int usuarioId,
        int page,
        int pageSize
    )
    {
        var query = _db
            .Notificaciones.Where(n => n.UsuarioDestinoId == usuarioId)
            .OrderByDescending(n => n.FechaCreacion);

        var total = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => n.ToDto())
            .ToListAsync();

        return new PaginacionResultado<NotificacionDto>
        {
            Items = items,
            PaginaActual = page,
            TotalPaginas = (int)Math.Ceiling(total / (double)pageSize),
            TotalRegistros = total,
        };
    }

    // // ------------------------------------------------------------
    // // Conversión a DTO
    // // ------------------------------------------------------------
    // private static NotificacionDto ToDto(Notificacion n) =>
    //     new()
    //     {
    //         Id = n.Id,
    //         UsuarioDestinoId = n.UsuarioDestinoId,
    //         UsuarioOrigenId = n.UsuarioOrigenId,
    //         Tipo = n.Tipo,
    //         PostId = n.PostId,
    //         ComentarioId = n.ComentarioId,
    //         Mensaje = n.Mensaje,
    //         FechaCreacion = n.FechaCreacion,
    //         Leida = n.Leida,
    //         Payload = n.Payload,
    //     };

    // ------------------------------------------------------------
    // Enviar email (método privado reutilizable)
    // ------------------------------------------------------------
    /* private async Task EnviarEmailNotificacion(Notificacion notificacion)
     {
         var emailDestino = await _db
             .Usuarios.Where(u => u.Id == notificacion.UsuarioDestinoId)
             .Select(u => u.Email)
             .FirstOrDefaultAsync();
 
         if (string.IsNullOrWhiteSpace(emailDestino))
             return;
 
         var asunto = $"Nueva notificación: {notificacion.Tipo}";
         var cuerpo =
             $@"
             <h2>Tienes una nueva notificación</h2>
             <p>{notificacion.Mensaje}</p>
             <p><small>Fecha: {notificacion.FechaCreacion}</small></p>
         ";
 
         await _email.EnviarAsync(emailDestino, asunto, cuerpo);
     }*/
}
