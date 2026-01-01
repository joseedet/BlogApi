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
    

    public ComentarioService(IComentarioRepository repo, INotificacionService notificacionService)
    {
        _repo = repo;
        _notificacionService = notificacionService;
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

        var postAutorId = await _context
            .Posts.Where(p => p.Id == comentario.PostId)
            .Select(p => p.UsuarioId)
            .FirstAsync(); // 🔥 Crear notificación

        await _notificacionService.CrearAsync(
            postAutorId,
            $"Tu post ha recibido un nuevo comentario de {comentario.UsuarioId}"
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
