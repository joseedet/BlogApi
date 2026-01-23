using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly BlogDbContext _context; 
    public RefreshTokenService(BlogDbContext context)
    { 
        _context = context;
    } 
    public RefreshToken GenerarRefreshToken(int usuarioId)
    { 
        return new RefreshToken { Token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64)), 
        UsuarioId = usuarioId, 
        Creado = DateTime.UtcNow, 
        Expira = DateTime.UtcNow.AddDays(7) // duración estándar }
        ; } 
    }
    public async Task GuardarRefreshTokenAsync(RefreshToken token)
    { _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();
    } 
    public async Task<RefreshToken?> ObtenerRefreshTokenAsync(string token)
    { return await _context.RefreshTokens 
            .Include(rt => rt.Usuario) 
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }
     public async Task RevocarRefreshTokenAsync(RefreshToken token, string? reemplazadoPor = null)
    { 
        token.Revocado = DateTime.UtcNow;
        token.ReemplazadoPor = reemplazadoPor;
         await _context.SaveChangesAsync();
          }
    public async Task RevocarTokensDelUsuarioAsync(int usuarioId)
    {
         var tokens = await _context.RefreshTokens
          .Where(rt => rt.UsuarioId == usuarioId && rt.Revocado == null)
          .ToListAsync(); 
          foreach (var t in tokens) t.Revocado = DateTime.UtcNow; 
          await _context.SaveChangesAsync();
     }
}

