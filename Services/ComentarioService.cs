using BlogApi.Data;
using BlogApi.Hubs;
using BlogApi.Models;
using BlogApi.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

public class ComentarioService : IComentarioService
{
    private readonly IComentarioRepository _repo;
    private readonly INotificacionService _notificacionService;
    private readonly BlogDbContext _context;
    private readonly IEmailService _emailService;
    private readonly EmailTemplateService _emailTemplateService = new();
    private readonly IHubContext<NotificacionesHub> _hub;

    /*public ComentarioService(IComentarioRepository repo, INotificacionService notificacionService)
    {
        _repo = repo;
        _notificacionService = notificacionService;
    }*/

    public ComentarioService(
        IComentarioRepository repo,
        BlogDbContext context,
        INotificacionService notificacionService,
        IEmailService emailService,
        IHubContext<NotificacionesHub> hub
    )
    {
        _repo = repo;
        _context = context;
        _notificacionService = notificacionService;
        _emailService = emailService;
        _hub = hub;
    }

    public async Task<IEnumerable<Comentario>> GetComentariosDePostAsync(int postId)
    {
        // Comentarios raíz con respuestas y usuario
        return await _repo
            .Query()
            .Where(c => c.PostId == postId && c.ComentarioPadreId == null)
            .Include(c => c.Usuario)
            .Include(c => c.Respuestas)
                .ThenInclude(r => r.Usuario)
            .Include(c => c.Respuestas)
                .ThenInclude(r => r.Respuestas)
            .ToListAsync();
    }

    // await _repo.GetByPostIdAsync(postId);

    public async Task<Comentario> CrearComentarioAsync(Comentario comentario)
    {
        comentario.Fecha = DateTime.UtcNow;
        comentario.Estado = "pendiente";

        await _repo.AddAsync(comentario);
        await _repo.SaveChangesAsync();

        var postAutor = await _context
            .Posts.Include(p => p.Usuario)
            // ← NECESARIO
            .Where(p => p.Id == comentario.PostId)
            .Select(p => new
            {
                p.UsuarioId,
                Email = p.Usuario.Email,
                Nombre = p.Usuario.Nombre,
                p.Titulo,
            })
            .FirstAsync();

        // Enviar notificación al autor del post

        var autorComentario = await _context
            .Usuarios.Where(u => u.Id == comentario.UsuarioId)
            .Select(u => u.Nombre)
            .FirstAsync();

        // Crear notificación en la base de datos
        await _notificacionService.CrearAsync(
            postAutor.UsuarioId,
            $"Tu post ha recibido un nuevo comentario de {autorComentario}"
        );
        // Enviar notificación en tiempo real vía SignalR
        await _hub
            .Clients.User(postAutor.UsuarioId.ToString())
            .SendAsync(
                "NuevaNotificacion",
                new
                {
                    mensaje = $"Tu post '{postAutor.Titulo}' ha recibido un nuevo comentario.",
                    fecha = DateTime.UtcNow,
                }
            );
        // Enviar email al autor del post
        var plantilla = _emailTemplateService.CargarPlantilla("NuevoComentario.html");
        var html = _emailTemplateService.ReemplazarVariables(
            plantilla,
            new Dictionary<string, string>
            {
                { "NOMBRE_USUARIO", postAutor.Nombre },
                { "TITULO_POST", postAutor.Titulo },
            }
        );
        // 🔥 Enviar email
        await _emailService.EnviarAsync(
            postAutor.Email,
            "Nuevo comentario en tu post",
            $"Has recibido un nuevo comentario en tu post: {postAutor.Titulo}"
        );

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

    /// <summary>
    /// Elimina un comentario por su ID
    /// </summary>
    /// <param name="comentarioId"></param>
    /// <param name="usuarioId"></param>
    /// <param name="puedeBorrarTodo"></param>
    /// <returns>bool</returns>
    public async Task<bool> EliminarComentarioAsync(
        int comentarioId,
        int usuarioId,
        bool puedeBorrarTodo
    )
    {
        var comentario = await _repo
            .Query()
            .Include(c => c.Respuestas)
            .FirstOrDefaultAsync(c => c.Id == comentarioId);
        if (comentario == null)
            return false;

        // Si no es admin/editor, solo puede borrar sus propios comentarios

        if (!puedeBorrarTodo && comentario.UsuarioId != usuarioId)
            return false;

        // Si tiene respuestas, las borramos también (estilo WordPress)

        if (comentario.Respuestas.Any())
        {
            foreach (var respuesta in comentario.Respuestas)
                _repo.Remove(respuesta);
        }
        _repo.Remove(comentario);
        await _repo.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Obtiene los comentarios por estado
    /// </summary>
    /// <param name="estado"></param>
    /// <returns>IEnumerable<Comentario></returns>
    public async Task<IEnumerable<Comentario>> GetByEstadoAsync(string estado)
    {
        return await _repo
            .Query()
            .Where(c => c.Estado == estado)
            .Include(c => c.Usuario)
            .Include(c => c.Respuestas)
            .ToListAsync();
    }
}
