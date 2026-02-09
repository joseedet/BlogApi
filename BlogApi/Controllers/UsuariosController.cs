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
    private readonly ILogService _logService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="service"></param>
    /// <param name="tokenService"></param>
    /// <param name="logService"></param>
    public UsuariosController(
        IUsuarioService service,
        ITokenService tokenService,
        ILogService logService
    )
    {
        _service = service;
        _tokenService = tokenService;
        _logService = logService;
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
        if (!ok)
            return NotFound("Usuario no encontrado");
        // Aquí podrías registrar un log administrativo
        await _logService.RegistrarAsync(GetUserId(), "BloquearUsuario", id);
        return Ok("Usuario bloqueado");
    }

    /// <summary>
    /// Desbloquea un usuario
    /// </summary>
    [Authorize(Policy = "Permiso:Usuarios.Bloquear")]
    [HttpPost("{id:int}/desbloquear")]
    public async Task<IActionResult> Desbloquear(int id)
    {
        var ok = await _service.DesbloquearAsync(id);
        if (!ok)
            return NotFound("Usuario no encontrado");
        // Aquí podrías registrar un log administrativo
        // await _logService.RegistrarAsync(GetUserId(), "DesbloquearUsuario", id);
        return Ok("Usuario desbloqueado");
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

    /// <summary>
    /// Filtra usuarios por rol, estado de bloqueo y búsqueda por nombre o email, con paginación (solo admin/panel)
    /// </summary>
    /// <param name="filtro"></param>
    /// <returns></returns>
    [Authorize(Policy = "Permiso:Usuarios.Ver")]
    [HttpGet("admin/filtrar")]
    public async Task<IActionResult> Filtrar([FromQuery] UsuarioFiltroDto filtro)
    {
        var result = await _service.FiltrarAsync(filtro);

        return Ok(
            new
            {
                result.PaginaActual,
                result.TotalPaginas,
                result.TotalRegistros,
                Usuarios = result.Items.Select(u => u.ToDto()),
            }
        );
    }

    /// <summary>
    /// Obtiene un usuario por ID (solo admin/panel)
    /// </summary>
    [Authorize(Policy = "Permiso:Usuarios.Ver")]
    [HttpGet("admin/{id:int}")]
    public async Task<IActionResult> GetByIdAdmin(int id)
    {
        var usuario = await _service.BuscarUsuarioPorIdAsync(id);

        if (usuario == null)
            return NotFound("Usuario no encontrado");

        return Ok(usuario.ToDto());
    }
    /// <summary>
    /// Edita un usuario (solo admin/panel)
    /// </summary>
    [Authorize(Policy = "Permiso:Usuarios.Editar")]
    [HttpPut("admin/{id:int}")]
    public async Task<IActionResult> EditarUsuarioAdmin(
        int id,
        [FromBody] EditarUsuarioAdminDto dto
    )
    {
        var usuario = await _service.BuscarUsuarioPorIdAsync(id);

        if (usuario == null)
            return NotFound("Usuario no encontrado");

        var ok = await _service.EditarUsuarioAdminAsync(id, dto);

        if (!ok)
            return BadRequest("No se pudo actualizar el usuario");

        // Registrar log administrativo
        await _logService.RegistrarAsync(GetUserId(), "EditarUsuario", id);

        return Ok("Usuario actualizado correctamente");
    }
    /// <summary>
    /// Asigna un rol a un usuario (solo admin/panel)
    /// </summary>
    [Authorize(Policy = "Permiso:Usuarios.Editar")]
    [HttpPost("admin/{id:int}/roles")]
    public async Task<IActionResult> AsignarRol(int id, [FromBody] AsignarRolDto dto)
    {
        var ok = await _service.AsignarRolAsync(id, dto.RolId);

        if (!ok)
            return BadRequest(
                "No se pudo asignar el rol. Verifica que el usuario y el rol existan."
            );

        await _logService.RegistrarAsync(GetUserId(), "AsignarRol", id, $"RolId: {dto.RolId}");

        return Ok("Rol asignado correctamente");
    }
    /// <summary>
    /// Quita un rol a un usuario (solo admin/panel)
    /// </summary>
    [Authorize(Policy = "Permiso:Usuarios.Editar")]
    [HttpDelete("admin/{id:int}/roles/{rolId:int}")]
    public async Task<IActionResult> QuitarRol(int id, int rolId)
    {
        var ok = await _service.QuitarRolAsync(id, rolId);

        if (!ok)
            return BadRequest("No se pudo quitar el rol. Verifica que el usuario tenga ese rol.");

        await _logService.RegistrarAsync(GetUserId(), "QuitarRol", id, $"RolId: {rolId}");

        return Ok("Rol eliminado correctamente");
    }
}
