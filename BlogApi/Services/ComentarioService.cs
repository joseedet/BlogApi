using BlogApi.Data;
using BlogApi.Domain.Factories;
using BlogApi.DTO;
using BlogApi.Hubs;
using BlogApi.Models;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services.Interfaces;
using BlogApi.Utils.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

/// <summary>
/// Servicio para gestionar comentarios
/// </summary>
//[Obsolete("NotificacionService está obsoleto. Usa NotificacionesService en su lugar.")]
public class ComentarioService : IComentarioService
{
    /// <summary>
    ///     Repositorio de comentarios
    /// </summary>
    private readonly IComentarioRepository _repo;

    /// <summary>
    ///   Servicio de notificaciones
    /// </summary>
    //[Obsolete]
    private readonly INotificacionesService _notificacionesService;

    /// <summary>
    ///     Contexto de la base de datos
    /// </summary>
    private readonly BlogDbContext _context;

    /// <summary>
    ///     Servicio de email
    /// </summary>
    private readonly IEmailService _emailService;

    /// <summary>
    ///     Servicio de plantillas de email
    /// </summary>
    private readonly IEmailTemplateService _emailTemplateService;

    /// <summary>
    ///    Hub de notificaciones para SignalR
    /// </summary>
    private readonly IHubContext<NotificacionesHub> _hub;

    private readonly IUsuarioService _usuarioService;

    /// <summary>
    /// Constructor de ComentarioService
    /// </summary>
    /// <param name="repo"></param>
    /// <param name="context"></param>
    /// <param name="notificacionService"></param>
    /// <param name="emailService"></param>
    /// <param name="hub"></param>
    //[Obsolete("NotificacionService está obsoleto. Usa NotificacionesService en su lugar.")]
    public ComentarioService(
        IComentarioRepository repo,
        BlogDbContext context,
        INotificacionesService notificacionService,
        IEmailService emailService,
        IHubContext<NotificacionesHub> hub,
        IEmailTemplateService emailTemplateService,
        IUsuarioService usuarioService
    )
    {
        _repo = repo;
        _context = context;
        _notificacionesService = notificacionService;
        _emailService = emailService;
        _hub = hub;
        _emailTemplateService = emailTemplateService;
        _usuarioService = usuarioService;
    }

    /// <summary>
    /// Obtiene los comentarios de un post
    /// </summary>
    /// <param name="postId"></param>
    /// <returns>IEnumerable&lt;Comentario&gt;</returns>
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
            .OrderByDescending(c => c.FechaCreacion)
            .ToListAsync();
    }

    // await _repo.GetByPostIdAsync(postId);
    /// <summary>
    /// Crea un nuevo comentario
    /// </summary>
    /// <param name="comentario"></param>
    /// <returns>Comentario</returns>
    public async Task<Comentario> CrearComentarioAsync(Comentario comentario)
    {
        // 1. Validar usuario
        var usuario = await _usuarioService.BuscarUsuarioPorIdAsync(comentario.UsuarioId!.Value);
        if (usuario == null)
            throw new UnauthorizedAccessException("El usuario no existe.");

        if (usuario.EstaBloqueado)
            throw new UnauthorizedAccessException("El usuario está bloqueado.");

        // 2. Validar post
        var post = await _context
            .Posts.Include(p => p.Usuario)
            .Where(p => p.Id == comentario.PostId)
            .Select(p => new
            {
                p.Id,
                p.Titulo,
                AutorId = p.UsuarioId,
                AutorEmail = p.Usuario.Email,
                AutorNombre = p.Usuario.Nombre,
            })
            .FirstOrDefaultAsync();

        if (post == null)
            throw new ArgumentException("El post no existe.");

        // 3. Crear comentario
        comentario.FechaCreacion = DateTime.UtcNow;
        comentario.Estado = ComentarioEstado.Pendiente;

        await _repo.AddAsync(comentario);
        await _repo.SaveChangesAsync();

        // 4. Crear notificación para el autor del post
        var notificacion = NotificacionFactory.NuevoComentario(
            usuarioDestinoId: post.AutorId,
            usuarioOrigenId: comentario.UsuarioId!.Value,
            postId: comentario.PostId,
            comentarioId: comentario.Id,
            contenido: comentario.Contenido
        );

        await _notificacionesService.CrearAsync(notificacion);

        // 5. Notificación en tiempo real vía SignalR
        await _hub
            .Clients.User(post.AutorId.ToString())
            .SendAsync(
                "NuevaNotificacion",
                new
                {
                    mensaje = $"Tu post '{post.Titulo}' ha recibido un nuevo comentario.",
                    FechaCreacion = DateTime.UtcNow,
                }
            );

        // 6. Enviar email al autor del post
        var plantilla = await _emailTemplateService.CargarPlantillaAsync("NuevoComentario.html");

        var html = _emailTemplateService.ReemplazarVariables(
            plantilla,
            new Dictionary<string, string>
            {
                { "NOMBRE_USUARIO", post.AutorNombre },
                { "TITULO_POST", post.Titulo },
            }
        );

        await _emailService.EnviarAsync(post.AutorEmail, "Nuevo comentario en tu post", html);

        return comentario;
    }

    /// <summary>
    ///     Cambia el estado de un comentario (Aprobar, Rechazar, Pendiente)
    /// Solo Admin/Editor pueden cambiar el estado
    /// Un usuario no puede cambiar el estado de su propio comentario
    /// El nuevo estado se pasa como string pero se convierte a enum internamente
    /// Devuelve true si se cambió el estado, false si no se encontró el comentario o el estado es inválido
    /// Lanza UnauthorizedAccessException si el usuario no existe o está bloqueado
    /// </summary>
    /// <param name="comentarioId"></param>
    /// <param name="usuarioId"></param>
    /// <param name="estado"></param>
    /// <returns>True si se cambió el estado, false si no se encontró el comentario o el estado es inválido</returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// </summary>
    public async Task<bool> CambiarEstadoAsync(
        int comentarioId,
        int usuarioId,
        ComentarioEstado nuevoEstado
    )
    {
        // Validar usuario
        var usuario = await _usuarioService.BuscarUsuarioPorIdAsync(usuarioId); // ✔ BIEN

        if (usuario == null || usuario.EstaBloqueado)
            throw new UnauthorizedAccessException("El usuario no está autorizado.");

        var comentario = await _repo
            .Query()
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(c => c.Id == comentarioId);
        // El autor NO puede moderar su propio comentario
        if (comentario.UsuarioId == usuarioId)
            throw new UnauthorizedAccessException("No puedes moderar tu propio comentario.");

        if (comentario == null)
            return false;

        //Cambiar el estado de las respuestas si el comentario es rechazado

        comentario.Estado = nuevoEstado;

        _repo.Update(comentario);
        await _repo.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Elimina un comentario por su ID
    /// </summary>
    /// <param name="comentarioId"></param>
    /// <param name="usuarioId"></param>
    /// <returns>true si se eliminó correctamente</returns>
    public async Task<bool> EliminarComentarioAsync(int comentarioId, int usuarioId)
    {
        var usuario = await _usuarioService.BuscarUsuarioPorIdAsync(usuarioId);
        if (usuario == null || usuario.EstaBloqueado)
            throw new UnauthorizedAccessException("El usuario no está autorizado.");

        var comentario = await _repo
            .Query()
            .Include(c => c.Respuestas)
            .FirstOrDefaultAsync(c => c.Id == comentarioId);

        if (comentario == null)
            return false;

        // 1. Si el usuario es el autor → puede borrar su comentario
        if (comentario.UsuarioId == usuarioId)
            throw new UnauthorizedAccessException(
                "No puedes eliminar comentarios de otros usuarios."
            );

        // Borrar respuestas
        foreach (var respuesta in comentario.Respuestas)
            _repo.Remove(respuesta);
        _repo.Remove(comentario);
        await _repo.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Obtiene los comentarios por estado
    /// </summary>
    /// <param name="estado"></param>
    /// <returns>IEnumerable&lt;Comentario&gt;</returns>
    public async Task<IEnumerable<Comentario>> GetByEstadoAsync(string estado)
    {
        if (!Enum.TryParse<ComentarioEstado>(estado, true, out var estadoEnum))
            throw new ArgumentException("Estado inválido.");

        return await _repo
            .Query()
            .Where(c => c.Estado == estadoEnum)
            .Include(c => c.Usuario)
            .Include(c => c.Respuestas)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene un comentario por su ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Comentario o null</returns>
    public async Task<Comentario?> GetByIdAsync(int id)
    {
        //return await _context.Comentarios.FindAsync(id);
        // o con Include si necesitas navegación:
        //return await _repo.Query().FirstOrDefaultAsync(c => c.Id == id);
        return await _repo
            .Query()
            .Include(c => c.Usuario)
            .Include(c => c.Respuestas)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <summary>
    /// Pagina de pendientes
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns>PaginacionResultado&lt;Comentario&gt;</returns>
    public async Task<PaginacionResultado<Comentario>> GetPendientesPaginadoAsync(
        int page,
        int pageSize
    )
    {
        var query = _repo
            .Query()
            .Where(c => c.Estado == ComentarioEstado.Pendiente)
            .OrderByDescending(c => c.FechaCreacion);

        var total = await query.CountAsync();

        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginacionResultado<Comentario>
        {
            Items = items,
            PaginaActual = page,
            TotalPaginas = (int)Math.Ceiling(total / (double)pageSize),
            TotalRegistros = total,
        };
    }
    /// <summary>
    /// Filtra comentarios según los criterios especificados en el DTO de filtro. Permite filtrar por ID de post, ID de usuario y estado del comentario, así como paginar los resultados.
    /// </summary>
    /// <param name="filtro"></param>
    /// <returns>PaginadoResultado&lt;Comentario&gt;</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<PaginacionResultado<Comentario>> FiltrarAsync(ComentarioFiltroDto filtro)
    {
        var query = _repo.Query().Include(c => c.Usuario).Include(c => c.Respuestas).AsQueryable();

        // Filtrar por post
        if (filtro.PostId.HasValue)
            query = query.Where(c => c.PostId == filtro.PostId.Value);

        // Filtrar por usuario
        if (filtro.UsuarioId.HasValue)
            query = query.Where(c => c.UsuarioId == filtro.UsuarioId.Value);

        // Filtrar por estado
        if (!string.IsNullOrWhiteSpace(filtro.Estado))
        {
            if (Enum.TryParse<ComentarioEstado>(filtro.Estado, true, out var estadoEnum))
                query = query.Where(c => c.Estado == estadoEnum);
            else
                throw new ArgumentException("Estado inválido.");
        }

        // Ordenar por fecha
        query = query.OrderByDescending(c => c.FechaCreacion);

        // Paginación
        var total = await query.CountAsync();

        var items = await query
            .Skip((filtro.Page - 1) * filtro.PageSize)
            .Take(filtro.PageSize)
            .ToListAsync();

        return new PaginacionResultado<Comentario>
        {
            Items = items,
            PaginaActual = filtro.Page,
            TotalPaginas = (int)Math.Ceiling(total / (double)filtro.PageSize),
            TotalRegistros = total,
        };
    }
}
