using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace BlogApi.Services;

/// <summary>
/// Servicio para la generación de tokens JWT
/// </summary>
public class TokenService : ITokenService
{
    /// <summary>
    /// Configuración de la aplicación
    /// </summary>
    private readonly IConfiguration _config;
    private readonly BlogDbContext _context;

    /// <summary>
    /// Constructor de TokenService
    /// </summary>
    /// <param name="config"></param>
    /// <param name="context"></param>
    public TokenService(IConfiguration config, BlogDbContext context)
    {
        _config = config;
        _context = context;
    }

    /// <summary>
    /// Genera un token JWT para un usuario
    /// </summary>
    /// <param name="usuario"></param>
    /// <returns>string</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public string GenerateToken(Usuario usuario)
    {
        var key = _config["Jwt:Key"];
        if (string.IsNullOrEmpty(key))
            throw new InvalidOperationException("JWT Key no configurada");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        // ------------------------------- // 1. Cargar roles del usuario // -------------------------------
        var roles = _context
            .UsuarioRoles.Where(ur => ur.UsuarioId == usuario.Id)
            .Select(ur => ur.Rol)
            .ToList();
        // ------------------------------- // 2. Cargar permisos de esos roles // -------------------------------
        var permisos = _context
            .RolPermisos.Where(rp => roles.Select(r => r.Id).Contains(rp.RolId))
            .Select(rp => rp.Permiso.Clave)
            .Distinct()
            .ToList();
        // ------------------------------- // 3. Claims base del usuario // -------------------------------
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
        };
        // ------------------------------- // 4. Añadir roles como claims // -------------------------------
        foreach (var rol in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, rol.Nombre));
        }
        // ------------------------------- // 5. Añadir permisos como claims // -------------------------------
        foreach (var permiso in permisos)
        {
            claims.Add(new Claim("permiso", permiso));
        }
        // ------------------------------- // 6. Generar token // -------------------------------
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
