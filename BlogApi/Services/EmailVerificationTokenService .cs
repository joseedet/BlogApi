using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services.Interfaces;

namespace BlogApi.Services;

/// <summary>
/// Email Verification Token Service
/// </summary>
public class EmailVerificationTokenService : IEmailVerificationTokenService
{
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly IEmailVerificationTokenRepository _tokenRepo;
    private readonly IEmailService _emailService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="usuarioRepo"></param>
    /// <param name="tokenRepo"></param>
    /// <param name="emailService"></param>
    // tu servicio real de email
    public EmailVerificationTokenService(
        IUsuarioRepository usuarioRepo,
        IEmailVerificationTokenRepository tokenRepo,
        IEmailService emailService
    )
    {
        _usuarioRepo = usuarioRepo;
        _tokenRepo = tokenRepo;
        _emailService = emailService;
    }

    // --------------------------------------------------------- // 1. GENERAR TOKEN + GUARDAR + ENVIAR EMAIL // ---------------------------------------------------------
    /// <summary>
    /// Genera el envio de token
    /// </summary>
    /// <param name="usuario"></param>
    /// <param name="ip"></param>
    /// <param name="userAgent"></param>
    /// <returns></returns>
    public async Task GenerarYEnviarTokenAsync(Usuario usuario, string ip, string userAgent)
    {
        // 1) Generar salt si no existe
        if (string.IsNullOrEmpty(usuario.EmailVerificationSalt))
        {
            usuario.EmailVerificationSalt = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            await _usuarioRepo.EstablecerSaltVerificacionAsync(
                usuario.Id,
                usuario.EmailVerificationSalt
            );
        }

        // 2) Generar token plano (solo para enviar por email)
        var tokenPlano = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

        // 3) Hashear token + salt
        var tokenHash = HashToken(tokenPlano, usuario.EmailVerificationSalt);

        // 4) Crear registro en tabla EmailVerificationTokens
        var token = new EmailVerificationToken
        {
            UserId = usuario.Id,

            // NO guardamos el token plano por seguridad
            Token = string.Empty, // o "***" si prefieres

            TokenHash = tokenHash,
            ExpiraEn = DateTime.UtcNow.AddHours(12),
            Usado = false,
            CreadoEn = DateTime.UtcNow,
            Reenvios = 0,
            IpCreacion = ip,
            UserAgentCreacion = userAgent,
        };

        await _tokenRepo.CrearAsync(token);

        // 5) Enviar email real con el token plano
        await _emailService.EnviarEmailVerificacionAsync(usuario.Email, tokenPlano);
    }

    // --------------------------------------------------------- // 2. VERIFICAR TOKEN // ---------------------------------------------------------
    /// <summary>
    /// Verifica el Token
    /// </summary>
    /// <param name="tokenPlano"></param>
    /// <param name="ip"></param>
    /// <param name="userAgent"></param>
    /// <returns></returns>
    public async Task<bool> VerificarTokenAsync(string tokenPlano, string ip, string userAgent)
    {
        // 1) Necesitamos obtener TODOS los usuarios que tengan salt
        var usuarios = await _usuarioRepo.GetAllAsync();

        foreach (var usuario in usuarios)
        {
            if (string.IsNullOrEmpty(usuario.EmailVerificationSalt))
                continue;

            // 2) Calcular hash(token + salt)
            var hash = HashToken(tokenPlano, usuario.EmailVerificationSalt);

            // 3) Buscar token por hash
            var token = await _tokenRepo.ObtenerPorHashAsync(hash);
            if (token == null)
                continue;

            // 4) Validar expiración y uso
            if (token.Usado || token.ExpiraEn < DateTime.UtcNow)
                return false;

            // 5) Marcar token como usado
            token.Usado = true;
            token.UsadoEn = DateTime.UtcNow;
            token.IpUso = ip;
            token.UserAgentUso = userAgent;

            await _tokenRepo.ActualizarAsync(token);

            // 6) Marcar usuario como verificado
            await _usuarioRepo.MarcarEmailVerificadoAsync(usuario.Id);

            return true;
        }

        return false;
    }

    // --------------------------------------------------------- // 3. REENVIAR TOKEN (máx 3 por hora) // ---------------------------------------------------------
    /// <summary>
    /// Reenvia el Token
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ip"></param>
    /// <param name="userAgent"></param>
    /// <returns></returns>
    public async Task<bool> ReenviarTokenAsync(int userId, string ip, string userAgent)
    {
        var usuario = await _usuarioRepo.GetByIdAsync(userId);
        if (usuario == null)
            return false;
        // 1) Control de reenvíos
        var reenvios = await _tokenRepo.ObtenerReenviosUltimaHoraAsync(userId);
        if (reenvios.Count() >= 3)
            return false;
        // 2) Obtener token activo o generar uno nuevo
        var tokenActivo = await _tokenRepo.ObtenerTokenActivoAsync(userId);
        if (tokenActivo == null)
        {
            await GenerarYEnviarTokenAsync(usuario, ip, userAgent);
            return true;
        }
        // 3) Reenviar token existente
        tokenActivo.Reenvios++;
        await _tokenRepo.ActualizarAsync(tokenActivo);
        await _emailService.EnviarEmailVerificacionAsync(usuario.Email, tokenActivo.Token);
        return true;
    }

    // --------------------------------------------------------- // MÉTODOS PRIVADOS // ---------------------------------------------------------

    private string HashToken(string token, string salt)
    {
        using var sha = SHA512.Create();
        var bytes = Encoding.UTF8.GetBytes(token + salt);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }

    /*  private async Task<Usuario?> BuscarUsuarioPorTokenPlano(string tokenPlano)
     {
         // Necesitamos recorrer todos los usuarios con salt
         // y comparar hash(token + salt)
         // pero optimizamos usando la tabla de tokens
         // 1) Obtener todos los tokens con ese TokenHash
         // (pero primero necesitamos el hash)
         // → No podemos calcularlo sin salt
         // → Así que buscamos por Token (tu modelo lo guarda)
         var tokens = await _tokenRepo.ObtenerTokensPorTokenPlano(tokenPlano);
      if (!tokens.Any()) return null; return tokens.First().Usuario;
       } private string HashToken(string token, string salt)
        {
         using var sha = SHA512.Create();
         var bytes = Encoding.UTF8.GetBytes(token + salt);
         var hash = sha.ComputeHash(bytes);
         return Convert.ToHexString(hash);
            }*/
}
