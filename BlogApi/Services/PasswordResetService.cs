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
public class PasswordResetService:IPasswordResetService
{
    private readonly BlogDbContext _context;
    private readonly IEmailService _emailService; // ya lo tienes
    private readonly IConfiguration _config;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context"></param>
    /// <param name="emailService"></param>
    /// <param name="config"></param>
    public PasswordResetService(
        BlogDbContext context,
        IEmailService emailService,
        IConfiguration config
    )
    {
        _context = context;
        _emailService = emailService;
        _config = config;
    }

    /// <summary>
    /// Solicitud para la recuperacion de la contraseña.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>

    public async Task SolicitarRecuperacionAsync(string email)
    { // 1. Buscar usuario por email
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        // Seguridad: no revelar si el email existe o no
        if (usuario == null)
        {
            // Opcional: log interno
            return;
        }
        // 2. Generar token aleatorio seguro
        var token = GenerarTokenSeguro();
        // 3. Hashear el token antes de guardarlo
        var tokenHash = CalcularHash(token);
        // 4. Crear registro en PasswordResetToken
        var resetToken = new PasswordResetToken
        {
            UsuarioId = usuario.Id,
            TokenHash = tokenHash,
            Creado = DateTime.UtcNow,
            Expira = DateTime.UtcNow.AddMinutes(30),
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
}
