using BlogApi.Data;
using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repositories;

/// <summary>
/// Implementación de IUsuario
/// </summary>
public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context"></param>
    public UsuarioRepository(BlogDbContext context)
        : base(context) { }

    /// <summary>
    /// Usuario por email
    /// </summary>
    /// <param name="email"></param>
    /// <returns>Usuario</returns>
    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        //return await context.Set<Usuario>().FirstOrDefaultAsync(u => u.Email == email);
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// Cuenta el numero de post por categoría
    /// </summary>
    /// <returns>Entero</returns>
    public Task<int> CountAsync()
    {
        return _dbSet.CountAsync();
    }

    /// <summary>
    /// Obtiene la actividad reciente de usuarios
    /// </summary>
    /// <param name="limit"></param>
    /// <returns>List&lt;Usuario&gt;</returns>
    public Task<List<Usuario>> GetRecentUsuariosAsync(int limit)
    {
        return _context.Usuarios.OrderByDescending(u => u.FechaRegistro).Take(limit).ToListAsync();
    }
}
