using BlogApi.DTO;
using BlogApi.Services;
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

        /// <summary>
        /// Crea un nuevo rol en la base de datos utilizando los datos proporcionados en el DTO CrearRolDto, devuelve el rol creado con su ID asignado, este método se utiliza para agregar nuevos roles al sistema desde la interfaz de administración o panel de control (solo accesible para usuarios con permisos de administración)
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Elimina un rol específico de la base de datos, verifica que el rol exista y que no sea un rol protegido antes de eliminarlo, devuelve un mensaje de éxito o error según corresponda, este método se utiliza para eliminar roles del sistema desde la interfaz de administración o panel de control (solo accesible para usuarios con permisos de administración)
        /// </summary>
        /// <param name="rolId"></param>
        /// <returns></returns>
        [Authorize(Policy = "Permiso:Usuarios.Editar")]
        [HttpDelete("{rolId:int}")]
        public async Task<IActionResult> EliminarRol(int rolId)
        {
            var ok = await _service.EliminarRolAsync(rolId);

            if (!ok)
                return BadRequest(
                    "No se pudo eliminar el rol. Verifica que exista y que no sea un rol protegido."
                );

            await _logService.RegistrarAsync(GetUserId(), "EliminarRol", rolId);

            return Ok("Rol eliminado correctamente");
        }

        /// <summary>
        /// Obtiene los detalles de un rol específico, incluyendo su nombre y los permisos asignados, devuelve un objeto con la información del rol o un mensaje de error si el rol no existe, este método se utiliza para mostrar la información de un rol en la interfaz de administración o panel de control (solo accesible para usuarios con permisos de administración)
        /// </summary>
        /// <param name="rolId"></param>
        /// <returns></returns>
        [Authorize(Policy = "Permiso:Usuarios.Editar")]
        [HttpGet("{rolId:int}")]
        public async Task<IActionResult> ObtenerDetalleRol(int rolId)
        {
            var rol = await _service.ObtenerDetalleRolAsync(rolId);

            if (rol == null)
                return NotFound("El rol no existe.");

            return Ok(rol);
        }
        /// <summary>
        /// Clona un rol existente, creando un nuevo rol con el mismo conjunto de permisos pero con un nuevo nombre y descripción proporcionados en el DTO ClonarRolDto, devuelve el nuevo rol creado o un mensaje de error si el rol original no existe o si el nuevo nombre ya está en uso, este método se utiliza para facilitar la creación de roles similares al permitir clonar un rol existente y luego modificar su nombre y descripción según sea necesario, solo accesible para usuarios con el permiso "Usuarios.Editar"
        /// </summary>
        /// <param name="rolId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Policy = "Permiso:Usuarios.Editar")]
        [HttpPost("{rolId:int}/clonar")]
        public async Task<IActionResult> ClonarRol(int rolId, [FromBody] ClonarRolDto dto)
        {
            var nuevoRol = await _service.ClonarRolAsync(rolId, dto);

            if (nuevoRol == null)
                return BadRequest(
                    "No se pudo clonar el rol. Verifica que exista o que el nuevo nombre no esté repetido."
                );

            await _logService.RegistrarAsync(GetUserId(), "ClonarRol", nuevoRol.Id);

            return Ok(new { mensaje = "Rol clonado correctamente", rolId = nuevoRol.Id });
        }
    }
}
