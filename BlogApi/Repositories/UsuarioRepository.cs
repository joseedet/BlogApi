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

    /// <summary>
    /// ¿Existe Email?
    /// </summary>
    /// <param name="email"></param>
    /// <param name="excludeUserId"></param>
    /// <returns>Devuelve verdadero si existe o falso en caso contrario</returns>
    public Task<bool> EmailExistsAsync(string email, int excludeUserId)
    {
        return _context.Usuarios.AnyAsync(u => u.Email == email && u.Id != excludeUserId);
    }
    // --------------------------------------------------------- // NUEVOS MÉTODOS PARA VERIFICACIÓN DE EMAIL // --------------------------------------------------------- 
    ///  <summary>
    /// Guarda o actualiza el salt único del usuario para verificación de email.
    /// </summary>
    public async Task EstablecerSaltVerificacionAsync(int userId, string salt)
    {
        var usuario = await _dbSet.FirstOrDefaultAsync(u => u.Id == userId);
        if (usuario == null) return; usuario.EmailVerificationSalt = salt;
        _dbSet.Update(usuario); await _context.SaveChangesAsync();
    }
    /// <summary> 
    /// Marca el email del usuario como verificado.
    /// </summary>
    public async Task MarcarEmailVerificadoAsync(int userId)
    {
        var usuario = await _dbSet.FirstOrDefaultAsync(u => u.Id == userId);
        if (usuario == null) return; usuario.EmailVerificado = true;
        usuario.EmailVerificadoEn = DateTime.UtcNow;
        _dbSet.Update(usuario);
        await _context.SaveChangesAsync();
    }
    /// <summary>
    /// Obtiene un usuario a partir del hash del token (SHA-512(token + salt)).
    /// </summary>
    public async Task<Usuario?> ObtenerPorTokenHashAsync(string tokenHash)
    {
        // El token hash NO está en Usuarios, sino en EmailVerificationTokens.
        // Por tanto, hacemos un join manual.
        return await _context.EmailVerificationTokens
        .Where(t => t.TokenHash == tokenHash)
        .Select(t => t.Usuario).FirstOrDefaultAsync();
    }
    /// <summary>
    /// Bloquear usuario
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Verdadero si se bloqueó correctamente, falso en caso contrario</returns>
     public async Task<bool> BloquearAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            return false;

        usuario.EstaBloqueado = true;
        await _context.SaveChangesAsync();
        return true;
    }
    /// <summary>
    /// Desbloquear usuario
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Verdadero si se desbloqueó correctamente, falso en caso contrario</returns>
    public async Task<bool> DesbloquearAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            return false;

        usuario.EstaBloqueado = false;
        await _context.SaveChangesAsync();
        return true;
    }
}
