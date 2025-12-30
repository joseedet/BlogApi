using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _service;
    private readonly ITokenService _tokenService;

    public UsuariosController(IUsuarioService service, ITokenService tokenService)
    {
        _service = service;
        _tokenService = tokenService;
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost("registro")]
    public async Task<IActionResult> Registrar(Usuario usuario)
    {
        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);
        usuario.Rol = RolUsuario.Suscriptor;
        // por seguridad
        var created = await _service.CrearUsuarioAsync(usuario);
        return Ok(created.ToDto());
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var usuario = await _service.GetByEmailAsync(request.Email);
        if (usuario == null)
            return Unauthorized("Usuario no encontrado");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
            return Unauthorized("Contraseña incorrecta");
        var token = _tokenService.GenerateToken(usuario);

        return Ok(new { token });
    }

    // El login lo haremos cuando implementemos JWT
}
