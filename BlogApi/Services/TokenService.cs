using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlogApi.Models;
using BlogApi.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace BlogApi.Services;

public class TokenService : ITokenService
{
    /// <summary>
    /// Configuración de la aplicación
    /// </summary>
    private readonly IConfiguration _config;

    /// <summary>
    /// Constructor de TokenService
    /// </summary>
    /// <param name="config"></param>
    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Genera un token JWT para un usuario
    /// </summary>
    /// <param name="usuario"></param>
    /// <returns>Token JWT generado</returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// </summary>
    public string GenerateToken(Usuario usuario)
    {
        var key = _config["Jwt:Key"];
        if (string.IsNullOrEmpty(key))
            throw new InvalidOperationException("JWT Key no configurada");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            // Id del usuario para luego usar new ClaimTypes.NameIdentifier
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            //Email
            new Claim(ClaimTypes.Email, usuario.Email),
            // Rol (para User.IsInRole("Administrador"), etc.)
            new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
        };
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(4),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
