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
            return false;

        var token = await _context
            .PasswordResetTokens.Where(t => t.UsuarioId == usuario.Id && t.Usado == null)
            .OrderByDescending(t => t.Creado)
            .FirstOrDefaultAsync();

        if (token == null)
            return false;

        if (token.Expira < DateTime.UtcNow)
            return false;

        var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(tokenPlano)));

        if (hash != token.TokenHash)
            return false;

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
            return false;

        var token = await _context
            .PasswordResetTokens.Where(t => t.UsuarioId == usuario.Id && t.Usado == null)
            .OrderByDescending(t => t.Creado)
            .FirstOrDefaultAsync();

        if (token == null)
            return false;

        if (token.Expira < DateTime.UtcNow)
            return false;

        var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(tokenPlano)));
        if (hash != token.TokenHash)
            return false;

        // Marcar token como usado
        token.Usado = DateTime.UtcNow;

        // Actualizar contraseña del usuario
        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(nuevaPassword);

        // Guardar cambios
        await _context.SaveChangesAsync();

        return true;
    }
}
