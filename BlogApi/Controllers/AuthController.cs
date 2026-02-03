using BlogApi.DTO;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IPasswordResetService _passwordResetService;

    //private readonly JwtService _jwtService;

    /// <summary>
    /// Constructor del controlador de autenticación
    /// </summary>
    /// <param name="usuarioService"></param>
    /// <param name="tokenService"></param>
    /// <param name="refreshTokenService"></param>
    /// <param name="passwordResetService"></param>
    public AuthController(
        IUsuarioService usuarioService,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService,
        IPasswordResetService passwordResetService
    )
    {
        _usuarioService = usuarioService;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
        _passwordResetService = passwordResetService;
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

        var usuario = result.Usuario!;

        // 1. Generar access token (JWT)
        var accessToken = _tokenService.GenerateToken(result.Usuario!);

        // 2. Generar refresh token
        var refreshToken = _refreshTokenService.GenerarRefreshToken(usuario.Id);

        // 3. Guardarlo en BD
        await _refreshTokenService.GuardarRefreshTokenAsync(refreshToken);

        // 4. Devolver ambos token

        var response = new LoginResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshToken.Token,
            Usuario = new
            {
                usuario.Id,
                usuario.Nombre,
                usuario.Email,
                Rol = usuario.Rol.ToString(),
            },
        };
        return Ok(response);
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

    /*  /// <summary>
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
     } */
    /// <summary>
    /// Logout
    /// /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutDto dto)
    {
        var token = await _refreshTokenService.ObtenerRefreshTokenAsync(dto.RefreshToken);

        if (token == null)
            return NotFound("Refresh token no encontrado.");

        if (!token.EstaActivo)
            return BadRequest("El token ya está revocado o expirado.");

        await _refreshTokenService.RevocarRefreshTokenAsync(token);

        return Ok("Sesión cerrada correctamente.");
    }

    /// <summary>
    /// Logout todos
    /// </summary>
    /// <returns></returns>
    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll()
    {
        // Requiere que el usuario esté autenticado
        var usuarioId = int.Parse(User.FindFirst("id")!.Value);

        await _refreshTokenService.RevocarTokensDelUsuarioAsync(usuarioId);

        return Ok("Sesión cerrada en todos los dispositivos.");
    }

    /// <summary>
    /// Refresh Token
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenDto dto)
    {
        var token = await _refreshTokenService.ObtenerRefreshTokenAsync(dto.RefreshToken);

        if (token == null)
            return Unauthorized("Refresh token inválido.");

        if (!token.EstaActivo)
            return Unauthorized("Refresh token expirado o revocado.");

        var usuario = token.Usuario;

        await _refreshTokenService.RevocarRefreshTokenAsync(token);

        var nuevoAccessToken = _tokenService.GenerateToken(usuario);
        var nuevoRefreshToken = _refreshTokenService.GenerarRefreshToken(usuario.Id);

        await _refreshTokenService.GuardarRefreshTokenAsync(nuevoRefreshToken);

        return Ok(new { token = nuevoAccessToken, refreshToken = nuevoRefreshToken.Token });
    }

    /// <summary>
    /// Solicitud para la recuperación de la contraseña.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> SolicitarRecuperacionPassword(
        [FromBody] SolicitarRecuperacionPasswordDto dto
    )
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest("Email es obligatorio");
        await _passwordResetService.SolicitarRecuperacionAsync(dto.Email);
        // Siempre devolvemos lo mismo, exista o no el email
        return Ok("Si el email existe, se ha enviado un enlace de recuperación");
    }

    /// <summary>
    /// Valida el Token
    /// </summary>
    ///<param name="dto"></param>
    [HttpPost("validate-reset-token")]
    public async Task<IActionResult> ValidateResetToken([FromBody] ValidateResetTokenDto dto)
    {
        var isValid = await _passwordResetService.ValidarTokenAsync(dto.Email, dto.Token);

        if (!isValid)
            return BadRequest(new { message = "Token inválido o expirado" });

        return Ok(new { message = "Token válido" });
    }

    /// <summary>
    /// Resetear contraseña
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var ok = await _passwordResetService.ResetPasswordAsync(
            dto.Email,
            dto.Token,
            dto.NuevaPassword
        );

        return ok
            ? Ok("Contraseña actualizada correctamente")
            : BadRequest("Token inválido o expirado");
    }
}
