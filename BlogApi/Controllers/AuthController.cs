using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly ITokenService _tokenService;

    public AuthController(IUsuarioService usuarioService, ITokenService tokenService)
    {
        _usuarioService = usuarioService;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var usuario = await _usuarioService.GetByEmailAsync(request.Email);
        if (usuario == null)
            return Unauthorized("Usuario no encontrado");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
            return Unauthorized("Contraseña incorrecta");

        var token = _tokenService.GenerateToken(usuario);

        return Ok(new { token });
    }

    /*public async Task<IActionResult> Login(LoginRequest request)
    {
        var usuario = await _usuarioService.GetByEmailAsync(request.Email);
        if (usuario == null)
            return Unauthorized("Usuario no encontrado");
        if (!BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
            return Unauthorized("Contraseña incorrecta");
        var token = _tokenService.GenerateToken(usuario);
        return Ok(new { token });
    }*/

    [HttpPost("registro")]
    public async Task<IActionResult> Registro(Usuario usuario)
    {
        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);
        var created = await _usuarioService.CrearUsuarioAsync(usuario);
        return Ok(created);
    }
}
