using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repositories;

/// <summary>
/// Email verification token
/// </summary>
public class EmailVerificationTokenRepository : IEmailVerificationTokenRepository
{
    private readonly BlogDbContext _context;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context"></param>
    public EmailVerificationTokenRepository(BlogDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Crea un nuevo token de verificación.
    /// </summary>
    /// <param name="token"></param>
    public async Task CrearAsync(EmailVerificationToken token)
    {
        await _context.EmailVerificationTokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Obtiene el token activo del usuario:
    /// - No usado
    /// - No expirado
    /// </summary>
    /// <param name="userId"></param>
    public async Task<EmailVerificationToken?> ObtenerTokenActivoAsync(int userId)
    {
        return await _context
            .EmailVerificationTokens.Where(t => t.UserId == userId)
            .Where(t => !t.Usado)
            .Where(t => t.ExpiraEn > DateTime.UtcNow)
            .OrderByDescending(t => t.CreadoEn)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Obtiene todos los reenvíos realizados en la última hora.
    /// </summary>
    /// <param name="userId"></param>
    public async Task<IEnumerable<EmailVerificationToken>> ObtenerReenviosUltimaHoraAsync(
        int userId
    )
    {
        var haceUnaHora = DateTime.UtcNow.AddHours(-1);

        return await _context
            .EmailVerificationTokens.Where(t => t.UserId == userId)
            .Where(t => t.Reenvios > 0)
            .Where(t =>
                t.CreadoEn >= haceUnaHora || (t.UsadoEn != null && t.UsadoEn >= haceUnaHora)
            )
            .ToListAsync();
    }

    /// <summary>
    /// Busca un token por su hash (SHA-512(token + salt)).
    /// </summary>
    /// <param name="tokenHash"></param>
    public async Task<EmailVerificationToken?> ObtenerPorHashAsync(string tokenHash)
    {
        return await _context
            .EmailVerificationTokens.Include(t => t.Usuario)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
    }

    /// <summary>
    /// Actualiza un token existente.
    /// </summary>
    /// <param name="token"></param>
    public async Task ActualizarAsync(EmailVerificationToken token)
    {
        _context.EmailVerificationTokens.Update(token);
        await _context.SaveChangesAsync();
    }
}
