using BlogApi.DTO;
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
    private readonly JwtService _jwtService;

    /// <summary>
    /// Constructor del controlador de autenticación
    /// </summary>
    /// <param name="usuarioService"></param>
    /// <param name="tokenService"></param>
    /// <param name="jwtService"></param>
    public AuthController(
        IUsuarioService usuarioService,
        ITokenService tokenService,
        JwtService jwtService
    )
    {
        _usuarioService = usuarioService;
        _tokenService = tokenService;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Inicia sesión con email y contraseña
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>IActionResult</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _usuarioService.LoginAsync(dto);
        if (!result.Success)
            return Unauthorized(result.Error);
        var token = _jwtService.GenerarToken(result.Usuario!);
        return Ok(
            new
            {
                mensaje = "Login correcto.",
                token,
                usuario = new
                {
                    result.Usuario!.Id,
                    result.Usuario.Nombre,
                    result.Usuario.Email,
                    Rol = result.Usuario.Rol.ToString(),
                },
            }
        );
    }

    /// <summary>
    /// Registra un nuevo usuario
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>IActionResult</returns>
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

    /// <summary>
    /// Verifica el email del usuario
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>IActionResult</returns>
    [HttpPost("verificar-email")]
    public async Task<IActionResult> VerificarEmail(VerificarEmailDto dto)
    {
        var ok = await _usuarioService.VerificarEmailAsync(dto.Token);

        if (!ok)
            return BadRequest("Token inválido o expirado.");

        return Ok("Correo verificado correctamente. Ya puedes iniciar sesión.");
    }
}
