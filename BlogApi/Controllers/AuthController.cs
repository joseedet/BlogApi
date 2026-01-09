using System.Security.Cryptography;
using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

/// <summary>
/// Controlador para la autenticación de usuarios
/// </summary>
[ApiController]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly ITokenService _tokenService;

    /// <summary>
    /// Constructor del controlador de autenticación
    /// </summary>
    /// <param name="usuarioService"></param>
    /// <param name="tokenService"></param>
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
    public async Task<IActionResult> Registro(RegistroDto dto)
    {
        var created = await _usuarioService.RegistrarUsuarioAsync(dto);
        if (created == null)
            return BadRequest("El correo ya está registrado.");
        // Aquí enviarías el email real Console.WriteLine($"TOKEN DE VERIFICACIÓN: 
        Console.WriteLine($"TOKEN DE VERIFICACIÓN: {created.VerificacionToken}");
          return Ok("Usuario registrado. Revisa tu correo para verificar la cuenta.");
    }
}
