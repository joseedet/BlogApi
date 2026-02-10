using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    /// <summary>
    /// Controlador para manejar operaciones relacionadas con roles de usuario, como listar roles disponibles (solo para admin/panel)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRolService _service;
        private readonly ILogService _logService;

        /// <summary>
        /// Constructor que recibe el servicio de roles a través de inyección de dependencias
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logService"></param>
        public RolesController(IRolService service, ILogService logService)
        {
            _service = service;
            _logService = logService;
        }

        /// <summary>
        /// Lista todos los roles disponibles (solo admin/panel)
        /// </summary>
        [Authorize(Policy = "Permiso:Usuarios.Editar")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _service.GetAllAsync();
            return Ok(roles);
        }

        /// <summary>
        /// Actualiza los permisos de un rol específico, recibe una lista de IDs de permisos que se asignarán al rol, elimina los permisos actuales del rol y asigna la nueva lista de permisos proporcionada, solo accesible para usuarios con el permiso "Usuarios.Editar"
        /// </summary>
        /// <param name="rolId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Policy = "Permiso:Usuarios.Editar")]
        [HttpPut("{rolId:int}/permisos")]
        public async Task<IActionResult> ActualizarPermisosRol(
            int rolId,
            [FromBody] EditarPermisosRolDto dto
        )
        {
            var ok = await _service.ActualizarPermisosRolAsync(rolId, dto.PermisosIds);

            if (!ok)
                return BadRequest("No se pudo actualizar los permisos del rol.");

            await _logService.RegistrarAsync(GetUserId(), "EditarPermisosRol", rolId);

            return Ok("Permisos del rol actualizados correctamente");
        }

        private int GetUserId()
        {
            return int.Parse(
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value
            );
        }

        [Authorize(Policy = "Permiso:Usuarios.Editar")]
        [HttpPost]
        public async Task<IActionResult> CrearRol([FromBody] CrearRolDto dto)
        {
            var rol = await _service.CrearRolAsync(dto);

            if (rol == null)
                return BadRequest(
                    "No se pudo crear el rol. Verifica que el nombre no esté repetido."
                );

            await _logService.RegistrarAsync(GetUserId(), "CrearRol", rol.Id);

            return Ok(new { mensaje = "Rol creado correctamente", rolId = rol.Id });
        }
    }
}
