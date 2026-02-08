using System.Security.Cryptography;
using BlogApi.Data;
using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

/// <summary>
/// Servicio para la gestión de usuarios
/// </summary>
public class UsuarioService : IUsuarioService
{
    /// <summary>
    ///     Repositorio de usuarios
    /// </summary>
    private readonly IUsuarioRepository _repo;

    private readonly IPasswordHasher<Usuario> _passwordHasher;

    /// <summary>
    /// Repositorio de usuarios
    /// </summary>
    private readonly BlogDbContext _context;

    /// <summary>
    /// Constructor de UsuarioService
    /// </summary>
    /// <param name="repo"></param>
    /// <param name="context"></param>
    /// <param name="passwordHasher"></param>
    public UsuarioService(
        IUsuarioRepository repo,
        BlogDbContext context,
        IPasswordHasher<Usuario> passwordHasher
    )
    {
        _repo = repo;
        _context = context;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Obtiene un usuario por su email
    /// </summary>
    /// <param name="email"></param>
    /// <returns>Usuario o null</returns>
    public async Task<Usuario?> GetByEmailAsync(string email) => await _repo.GetByEmailAsync(email);

    /// <summary>
    /// Crea un nuevo usuario
    /// </summary>
    public async Task<Usuario> CrearUsuarioAsync(Usuario usuario)
    {
        /* if (usuario.Roles == 0)
             usuario.Rol = RolUsuario.Suscriptor;*/
        await _repo.AddAsync(usuario);
        await _repo.SaveChangesAsync();
        return usuario;
    }

    /// <summary>
    /// Registra un nuevo usuario
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>Usuario registrado o null si el email ya existe</returns>
    public async Task<Usuario> RegistrarUsuarioAsync(RegistroDto dto)
    {
        var email = dto.Email.Trim().ToLower();
        if (await _context.Usuarios.AnyAsync(u => u.Email == email))
            return null;
        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FechaRegistro = DateTime.UtcNow,
            EmailVerificado = false,
            /*VerificacionToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)),
            VerificacionTokenExpira = DateTime.UtcNow.AddHours(24),*/

            //Rol = RolUsuario.Suscriptor,
        };
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    /// <summary>
    /// Verifica el email del usuario
    /// </summary>
    /// <param name="token"></param>
    /// <returns>Verdadero si se verificó correctamente, falso en caso contrario</returns>
    /* public async Task<bool> VerificarEmailAsync(string token)
     {
         var usuario = await _context.Usuarios.FirstOrDefaultAsync(u =>
             u.VerificacionToken == token
         );
 
         if (usuario == null)
             return false;
 
         if (
             usuario.VerificacionTokenExpira == null
             || usuario.VerificacionTokenExpira < DateTime.UtcNow
         )
             return false;
 
         usuario.EmailVerificado = true;
         usuario.VerificacionToken = null;
         usuario.VerificacionTokenExpira = null;
 
         await _context.SaveChangesAsync();
         return true;
     }*/

    /// <summary>
    /// Inicia sesión con email y contraseña
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>Resultado del intento de login</returns>
    public async Task<LoginResult> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

        if (usuario.EstaBloqueado)
            return LoginResult.Failed("El usuario está bloqueado.");

        if (usuario == null)
            return LoginResult.Failed("Credenciales inválidas.");

        // ¿Está bloqueado?
        if (usuario.BloqueadoHasta != null && usuario.BloqueadoHasta > DateTime.UtcNow)
        {
            return LoginResult.Failed(
                "La cuenta está bloqueada temporalmente por intentos fallidos."
            );
        }

        // ¿Correo verificado?
        if (!usuario.EmailVerificado)
        {
            return LoginResult.Failed("Debes verificar tu correo antes de iniciar sesión.");
        }

        // Verificar contraseña
        var passwordOk = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash);

        if (!passwordOk)
        {
            usuario.IntentosFallidos++;

            // Límite de intentos fallidos
            if (usuario.IntentosFallidos >= 5)
            {
                usuario.BloqueadoHasta = DateTime.UtcNow.AddMinutes(15);
            }

            await _context.SaveChangesAsync();
            return LoginResult.Failed("Credenciales inválidas.");
        }

        // Login correcto: resetear contador y bloqueo
        usuario.IntentosFallidos = 0;
        usuario.BloqueadoHasta = null;

        await _context.SaveChangesAsync();

        return LoginResult.Ok(usuario);
    }

    /// <summary>
    /// Actualizar el perfil
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dto"></param>
    /// <returns>Devuelve verdadero si se ha podido o falso en caso contrario</returns>
    public async Task<bool> ActualizarPerfilAsync(int userId, ActualizarPerfilDto dto)
    {
        var usuario = await _repo.GetByIdAsync(userId);
        if (usuario == null)
            return false;

        // Validación: email único
        if (await _repo.EmailExistsAsync(dto.Email, userId))
            return false;

        usuario.Nombre = dto.Nombre;
        usuario.Apellidos = dto.Apellidos;
        usuario.Email = dto.Email;

        _repo.Update(usuario);
        await _repo.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Cambio de contraseña
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dto"></param>
    /// <returns>Devuelve verdadero si se ha podido cambiar en caso contrario devuelve falso.</returns>
    public async Task<bool> CambiarPasswordAsync(int userId, CambiarPasswordDto dto)
    {
        var usuario = await _repo.GetByIdAsync(userId);
        if (usuario == null)
            return false;

        // Validar contraseña actual
        var resultado = _passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.PasswordHash,
            dto.PasswordActual
        );

        if (resultado == PasswordVerificationResult.Failed)
            return false;

        // Generar nuevo hash
        usuario.PasswordHash = _passwordHasher.HashPassword(usuario, dto.NuevaPassword);

        _repo.Update(usuario);
        await _repo.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Subimos avatar
    /// </summary>
    /// <param name="userId"></param>
    /// /// <param name="avatar"></param>
    /// <returns>string</returns>
    /// <exception cref="Exception"></exception>
    public async Task<string> SubirAvatarAsync(int userId, IFormFile avatar)
    {
        var usuario = await _repo.GetByIdAsync(userId);
        if (usuario == null)
            return null;

        // Validaciones de seguridad
        var permitidos = new[] { "image/jpeg", "image/png", "image/webp" };
        if (!permitidos.Contains(avatar.ContentType))
            throw new Exception("Formato de imagen no permitido.");

        if (avatar.Length > 2 * 1024 * 1024)
            throw new Exception("El avatar no puede superar los 2MB.");

        // Guardar archivo
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(avatar.FileName)}";
        var path = Path.Combine("wwwroot/avatars", fileName);

        using (var stream = new FileStream(path, FileMode.Create))
            await avatar.CopyToAsync(stream);

        usuario.AvatarUrl = $"/avatars/{fileName}";

        _repo.Update(usuario);
        await _repo.SaveChangesAsync();

        return usuario.AvatarUrl;
    }

    /// <summary>
    /// Bloquear usuario
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Verdadero si se bloqueó correctamente, falso en caso contrario</returns>
    public async Task<bool> BloquearAsync(int id)
    {
        return await _repo.BloquearAsync(id);
    }

    /// <summary>
    /// Desbloquear usuario
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Verdadero si se desbloqueó correctamente, falso en caso contrario</returns>
    public async Task<bool> DesbloquearAsync(int id)
    {
        return await _repo.DesbloquearAsync(id);
    }
    /// <summary>
    /// Obtiene todos los usuarios
    /// </summary>
    /// <returns>IEnumerable&lt;Usuario&gt;</returns>
    public async Task<IEnumerable<Usuario>> GetAllAsync()
    {
        return await _context.Usuarios.ToListAsync();
    }
}
