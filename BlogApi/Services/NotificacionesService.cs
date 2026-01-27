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
    private readonly AppSettings _settings;
    private readonly INotificationSettingsService _notificationSettingsService;
    private readonly IUserNotificationPreferencesService _userNotificationPreferencesService;

    /// <summary>
    /// Constructor NotificacionesService
    /// </summary>
    /// <param name="db"></param>
    /// <param name="hub"></param>
    /// <param name="email"></param>
    /// <param name="emailTemplateService"></param>
    /// <param name="logger"></param>
    /// <param name="settings"></param>
    /// <param name="notificationSettingsService"></param>
    /// <param name="userNotificationPreferencesService"></param>
    public NotificacionesService(
        BlogDbContext db,
        IHubContext<NotificacionesHub> hub,
        IEmailService email,
        ILogger<NotificacionesService> logger,
        IEmailTemplateService emailTemplateService,
        IOptions<AppSettings> settings,
        INotificationSettingsService notificationSettingsService,
        IUserNotificationPreferencesService userNotificationPreferencesService
    )
    {
        _db = db;
        _hub = hub;
        _email = email;
        _logger = logger;
        _emailTemplateService = emailTemplateService;
        _settings = settings.Value;
        _notificationSettingsService = notificationSettingsService;
        _userNotificationPreferencesService = userNotificationPreferencesService;
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
            var global = await _notificationSettingsService.GetActiveAsync();

            // 1. Configuración global
            if (!global.SendEmailOnComment)
                return;

            // 2. Email (solo si aplica)
            await EnviarEmailNotificacion(notificacion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando notificación");
        }
    }

    /*private async Task EnviarEmailNotificacion(Notificacion notificacion)
    {
        // 1. Obtener configuración dinámica
        var config = await _notificationSettingsService.GetActiveAsync();

        // 2. Verificar si se debe enviar correo por comentario
        if (!config.SendEmailOnComment)
        {
            _logger.LogInformation("El envío de email por comentario está desactivado.");
            return;
        }
        // 3. Obtener datos necesarios
        var usuario = await _db.Usuarios.FindAsync(notificacion.UsuarioDestinoId);
        var post = await _db.Posts.FindAsync(notificacion.PostId);

        if (usuario == null || post == null)
            return;

        // 4. Construir modelo del correo
        var model = new CommentNotificationEmailModel
        {
            UserName = usuario.Nombre,
            PostTitle = post.Titulo,
            Email = usuario.Email,
        };

        // 5. Variables para la plantilla
        var variables = new Dictionary<string, string>
        {
            { "USER_NAME", model.UserName },
            { "POST_TITLE", model.PostTitle },
            { "APP_NAME", _settings.Value.AppName },
            { "SUBJECT", "Nuevo comentario en tu post" },
        };

        // 6. Renderizar plantilla completa
        var html = await _emailTemplateService.RenderTemplateAsync(
            "Notifications/comment-notification.html",
            variables
        );

        // 7. Enviar correo
        await _email.EnviarAsync(model.Email, "Nuevo comentario en tu post", html);
    }*/
    private async Task EnviarEmailNotificacion(Notificacion notificacion)
    {
        var global = await _notificationSettingsService.GetActiveAsync();
        var prefs = await _userNotificationPreferencesService.GetByUserIdAsync(
            notificacion.UsuarioDestinoId
        );

        if (!DebeEnviarEmail(notificacion.Tipo, global, prefs))
        {
            _logger.LogInformation(
                "Email no enviado por configuración global o preferencias del usuario."
            );
            return;
        }

        var usuario = await _db.Usuarios.FindAsync(notificacion.UsuarioDestinoId);
        if (usuario == null)
            return;

        var variables = new Dictionary<string, string>
        {
            { "USER_NAME", usuario.Nombre },
            { "MESSAGE", notificacion.Mensaje },
            { "APP_NAME", _settings.AppName },
            { "SUBJECT", "Nueva notificación" },
        };

        var html = await _emailTemplateService.RenderTemplateAsync(
            "Notifications/generic-notification.html",
            variables
        );

        await _email.EnviarAsync(usuario.Email, "Nueva notificación", html);
    }

    private bool DebeEnviarEmail(
        TipoNotificacion tipo,
        NotificationSettings global,
        UserNotificationPreferences prefs
    )
    {
        // 1. Preferencias globales del administrador
        switch (tipo)
        {
            case TipoNotificacion.RespuestaComentario:
            case TipoNotificacion.ComentarioEnPost:
            case TipoNotificacion.RespuestaAComentario:
            case TipoNotificacion.NuevoComentario:
                if (!global.SendEmailOnComment)
                    return false;
                break;

            case TipoNotificacion.MensajePrivado:
                if (!global.SendEmailOnAdminMessage)
                    return false;
                break;

            case TipoNotificacion.Sistema:
                if (!global.SendEmailOnSystemAlert)
                    return false;
                break;
        }

        // 2. Preferencias del usuario
        if (!prefs.ReceiveEmailNotifications)
            return false;

        switch (tipo)
        {
            case TipoNotificacion.RespuestaComentario:
            case TipoNotificacion.ComentarioEnPost:
            case TipoNotificacion.RespuestaAComentario:
            case TipoNotificacion.NuevoComentario:
                return prefs.NotifyOnComment;

            case TipoNotificacion.MensajePrivado:
                return prefs.NotifyOnAdminMessage;

            case TipoNotificacion.Sistema:
                return prefs.NotifyOnSystemAlert;

            default:
                return true; // Tipos no configurables aún
        }
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
}
