using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    /// <summary>
    /// Controlador para manejar operaciones relacionadas con permisos de usuario, como listar permisos disponibles (solo para admin/panel)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PermisosController : ControllerBase
    {
        private readonly IPermisoService _service;
        private readonly ILogService _logService;

        /// <summary>
        /// Constructor que recibe el servicio de permisos a través de inyección de dependencias
        /// </summary>
        /// <param name="service"></param>/
        /// <param name="logService"></param>
        public PermisosController(IPermisoService service, ILogService logService)
        {
            _service = service;
            _logService = logService;
        }

        /// <summary>
        /// Lista todos los permisos disponibles (solo admin/panel)
        /// </summary>
        [Authorize(Policy = "Permiso:Usuarios.Editar")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var permisos = await _service.GetAllAsync();
            return Ok(permisos);
        }
        /// <summary>
        /// Crea un nuevo permiso en la base de datos utilizando los datos proporcionados en el DTO CrearPermisoDto, devuelve el permiso creado con su ID asignado, este método se utiliza para agregar nuevos permisos al sistema desde la interfaz de administración o panel de control (solo accesible para usuarios con permisos de administración)
        /// </summary> <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Policy = "Permiso:Usuarios.Editar")]
        [HttpPost]
        public async Task<IActionResult> CrearPermiso([FromBody] CrearPermisoDto dto)
        {
            var permiso = await _service.CrearPermisoAsync(dto);

            if (permiso == null)
                return BadRequest(
                    "No se pudo crear el permiso. Verifica que la clave no esté repetida."
                );

            await _logService.RegistrarAsync(GetUserId(), "CrearPermiso", permiso.Id);

            return Ok(new { mensaje = "Permiso creado correctamente", permisoId = permiso.Id });
        }
        private int GetUserId()
        {
            return int.Parse(
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value
            );
        }
        /// <summary>
        /// Edita un permiso existente en la base de datos utilizando los datos proporcionados en el DTO EditarPermisoDto, devuelve un booleano indicando si la operación fue exitosa o no, este método se utiliza para actualizar los permisos existentes en el sistema desde la interfaz de administración o panel de control (solo accesible para usuarios con permisos de administración)
        /// </summary>
        /// <param name="permisoId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Policy = "Permiso:Usuarios.Editar")]
        [HttpPut("{permisoId:int}")]
        public async Task<IActionResult> EditarPermiso(
            int permisoId,
            [FromBody] EditarPermisoDto dto
        )
        {
            var ok = await _service.EditarPermisoAsync(permisoId, dto);

            if (!ok)
                return BadRequest(
                    "No se pudo editar el permiso. Verifica que exista o que la clave no esté repetida."
                );

            await _logService.RegistrarAsync(GetUserId(), "EditarPermiso", permisoId);

            return Ok("Permiso actualizado correctamente");
        }
    }
}
