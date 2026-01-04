using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

public class ComentarioService : IComentarioService
{
    private readonly IComentarioRepository _repo;
    private readonly INotificacionService _notificacionService;
    private readonly BlogDbContext _context;
    private readonly IEmailService _emailService;

    /*public ComentarioService(IComentarioRepository repo, INotificacionService notificacionService)
    {
        _repo = repo;
        _notificacionService = notificacionService;
    }*/

    public ComentarioService(
        IComentarioRepository repo,
        BlogDbContext context,
        INotificacionService notificacionService,
        IEmailService emailService
    )
    {
        _repo = repo;
        _context = context;
        _notificacionService = notificacionService;
        _emailService = emailService;
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
                p.Titulo,
            })
            .FirstAsync();

        /* var postAutorId = await _context
             .Posts.Where(p => p.Id == comentario.PostId)
             .Select(p => p.UsuarioId)
             .FirstAsync();*/
        // 🔥 Crear notificación

        var autorComentario = await _context
            .Usuarios.Where(u => u.Id == comentario.UsuarioId)
            .Select(u => u.Nombre)
            .FirstAsync();

        await _notificacionService.CrearAsync(
            postAutor.UsuarioId,
            $"Tu post ha recibido un nuevo comentario de {autorComentario}"
        );
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
