using System.Security.Claims;
using BlogApi.DTO;
using BlogApi.Mapper;
using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

/// <summary>
/// Controlador de usuario
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _service;
    private readonly ITokenService _tokenService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="service"></param>
    /// <param name="tokenService"></param>
    public UsuariosController(IUsuarioService service, ITokenService tokenService)
    {
        _service = service;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Registro usuario
    /// </summary>
    /// <param name="usuario"></param>
    /// <returns></returns>
    //[Authorize(Roles = "Administrador")]
    [HttpPost("registro")]
    public async Task<IActionResult> Registrar(Usuario usuario)
    {
        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);
        //usuario.Rol = RolUsuario.Suscriptor;
        // por seguridad
        var created = await _service.CrearUsuarioAsync(usuario);
        return Ok(created.ToDto());
    }

    /// <summary>
    /// Login de usuario
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _service.LoginAsync(dto);
        if (!result.Success)
            return Unauthorized(result.Error);
        var token = _tokenService.GenerateToken(result.Usuario);
        return Ok(new { token });
    }

    // El login lo haremos cuando implementemos JWT

    /// <summary>
    /// Actualiza el perfil
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPut("perfil")]
    public async Task<IActionResult> ActualizarPerfil([FromBody] ActualizarPerfilDto dto)
    {
        var ok = await _service.ActualizarPerfilAsync(GetUserId(), dto);
        return ok ? Ok("Perfil actualizado") : BadRequest("No se pudo actualizar el perfil");
    }

    /// <summary>
    /// Cambia el password
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPut("cambiar-password")]
    public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordDto dto)
    {
        var ok = await _service.CambiarPasswordAsync(GetUserId(), dto);
        return ok ? Ok("Contraseña actualizada") : BadRequest("Contraseña actual incorrecta");
    }

    /// <summary>
    /// Sube el avatar
    /// </summary>
    /// <param name="avatar"></param>
    /// <returns></returns>
    [HttpPost("avatar")]
    public async Task<IActionResult> SubirAvatar([FromForm] IFormFile avatar)
    {
        var url = await _service.SubirAvatarAsync(GetUserId(), avatar);
        return Ok(new { AvatarUrl = url });
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
    }

    /// <summary>
    /// Bloquea un usuario
    /// </summary>
    [Authorize(Policy = "Permiso:Usuarios.Bloquear")]
    [HttpPost("{id:int}/bloquear")]
    public async Task<IActionResult> Bloquear(int id)
    {
        var ok = await _service.BloquearAsync(id);
        return ok ? Ok("Usuario bloqueado") : NotFound("Usuario no encontrado");
    }

    /// <summary>
    /// Desbloquea un usuario
    /// </summary>
    [Authorize(Policy = "Permiso:Usuarios.Bloquear")]
    [HttpPost("{id:int}/desbloquear")]
    public async Task<IActionResult> Desbloquear(int id)
    {
        var ok = await _service.DesbloquearAsync(id);
        return ok ? Ok("Usuario desbloqueado") : NotFound("Usuario no encontrado");
    }

    /// <summary>
    /// Lista todos los usuarios (solo admin/panel)
    /// </summary>
    [Authorize(Policy = "Permiso:Usuarios.Ver")]
    [HttpGet("admin")]
    public async Task<IActionResult> GetAllAdmin()
    {
        var usuarios = await _service.GetAllAsync();
        return Ok(usuarios.Select(u => u.ToDto()));
    }
}
