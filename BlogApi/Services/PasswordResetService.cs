using System;
using System.Security.Cryptography;
using System.Text;
using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

/// <summary>
/// Implementación de la interfaz IPasswordResetService
/// </summary>
public class PasswordResetService : IPasswordResetService
{
    private readonly BlogDbContext _context;
    private readonly IEmailService _emailService; // ya lo tienes
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly ILogger<PasswordResetService> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context"></param>
    /// <param name="emailService"></param>
    /// <param name="config"></param>
    /// <param name="logger"></param>
    /// <param name="httpContextAccessor"></param>
    public PasswordResetService(
        BlogDbContext context,
        IEmailService emailService,
        IConfiguration config,
        ILogger<PasswordResetService> logger,
        IHttpContextAccessor httpContextAccessor


    )
    {
        _context = context;
        _emailService = emailService;
        _config = config;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Solicitud para la recuperacion de la contraseña.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task SolicitarRecuperacionAsync(string email)
    { // 1. Buscar usuario por email
        _logger.LogInformation("Solicitud de recuperación de contraseña para {Email}", email);
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        // Seguridad: no revelar si el email existe o no
        // Anti-enumeración: no revelamos si existe o no 
        if (usuario == null)
        {
            _logger.LogWarning("Solicitud de recuperación para email no registrado: {Email}", email);
            return;
        }
        // Rate limiting: no más de una solicitud cada 5 minutos 
        var ultimoToken = await _context.PasswordResetTokens.Where(t => t.UsuarioId == usuario.Id)
         .OrderByDescending(t => t.Creado)
         .FirstOrDefaultAsync();
          if (ultimoToken != null && ultimoToken.Creado > DateTime.UtcNow.AddMinutes(-5)) 
          {
            _logger.LogWarning("Rate limit de recuperación para {Email}", email);
            return;
         }
        // 2. Generar token aleatorio seguro
        var token = GenerarTokenSeguro();
        // 3. Hashear el token antes de guardarlo
        var tokenHash = CalcularHash(token);
        // 4. Crear registro en PasswordResetToken
        var ip = _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        var userAgent = _httpContextAccessor
            ?.HttpContext?.Request?.Headers["User-Agent"]
            .ToString();
        var resetToken = new PasswordResetToken
        {
            UsuarioId = usuario.Id,
            TokenHash = tokenHash,
            Creado = DateTime.UtcNow,
            Expira = DateTime.UtcNow.AddMinutes(30),
            Usado = null,
            IpCreacion = ip,
            UserAgentCreacion = userAgent
            // configurable
        };
        _context.PasswordResetTokens.Add(resetToken);
        await _context.SaveChangesAsync();
        // 5. Construir enlace de recuperación
        var frontendUrl = _config["Frontend:BaseUrl"];
        // ej: https://midominio.com
        var resetUrl = $"{frontendUrl}/reset-password?token={token}";
        // 6. Enviar email
        await _emailService.EnviarEmailRecuperacionPasswordAsync(usuario.Email, resetUrl);
        _logger.LogInformation("Email de recuperación enviado correctamente a {Email}", usuario.Email);
    }

    private string GenerarTokenSeguro()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }

    private string CalcularHash(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Valida el Token
    /// </summary>
    /// <param name="email"></param>
    /// <param name="tokenPlano"></param>
    /// <returns>Devuelve verdadero si está ok en caso contrario falso</returns>
    public async Task<bool> ValidarTokenAsync(string email, string tokenPlano)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

        if (usuario == null)
        {
            _logger.LogWarning(
                "Validación de token fallida: usuario no encontrado para {Email}",
                email
            );
            return false;
        }

        var token = await _context
            .PasswordResetTokens.Where(t => t.UsuarioId == usuario.Id && t.Usado == null)
            .OrderByDescending(t => t.Creado)
            .FirstOrDefaultAsync();

        if (token == null)
        {
            _logger.LogWarning(
                "Validación de token fallida: no hay token activo para {Email}",
                email
            );
            return false;
        }

        if (token.Expira < DateTime.UtcNow)
        {
            _logger.LogWarning("Validación de token fallida: token expirado para {Email}", email);
            return false;
        }

        var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(tokenPlano)));

        if (hash != token.TokenHash)
        {
            _logger.LogWarning("Validación de token fallida: hash no coincide para {Email}", email);
            return false;
        }
        _logger.LogInformation("Token válido para {Email}", email);
        return true;
    }

    /// <summary>
    /// Resetear contraseña
    /// </summary>
    /// <param name="email"></param>
    /// <param name="tokenPlano"></param>
    /// <param name="nuevaPassword"></param>
    /// <returns>Devuelve verdadero si se ha podido en caso contrario falso</returns>
    public async Task<bool> ResetPasswordAsync(
        string email,
        string tokenPlano,
        string nuevaPassword
    )
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

        if (usuario == null)
        {
            _logger.LogWarning("Reset password fallido: usuario no encontrado para {Email}", email);
            return false;
        }

        var token = await _context
            .PasswordResetTokens.Where(t => t.UsuarioId == usuario.Id && t.Usado == null)
            .OrderByDescending(t => t.Creado)
            .FirstOrDefaultAsync();

        if (token == null)
        {
            _logger.LogWarning("Reset password fallido: no hay token activo para {Email}", email);
            return false;
        }
        if (token.Expira < DateTime.UtcNow)
        {
            _logger.LogWarning("Reset password fallido: token expirado para {Email}", email);
            return false;
        }

        var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(tokenPlano)));
        if (hash != token.TokenHash)
        {
            _logger.LogWarning("Reset password fallido: hash no coincide para {Email}", email);
            return false;
        }
        var ip = _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        var userAgent = _httpContextAccessor
            ?.HttpContext?.Request?.Headers["User-Agent"]
            .ToString();

        token.Usado = DateTime.UtcNow;
        token.IpUso = ip;
        token.UserAgentUso = userAgent;

        // Invalidar todos los tokens activos del usuario
        var tokens = await _context
            .PasswordResetTokens.Where(t => t.UsuarioId == usuario.Id && t.Usado == null)
            .ToListAsync();
        var ahora = DateTime.UtcNow;
        foreach (var t in tokens)
        {
            t.Usado = ahora;
            t.IpUso = ip;
            t.UserAgentUso = userAgent;
        }

        // Actualizar contraseña del usuario
        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(nuevaPassword);

        // Guardar cambios
        await _context.SaveChangesAsync();

        _logger.LogInformation("Contraseña reseteada correctamente para {Email}", email);
        return true;
    }
}
