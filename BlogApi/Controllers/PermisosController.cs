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

        /// <summary>
        /// Constructor que recibe el servicio de permisos a través de inyección de dependencias
        /// </summary>
        /// <param name="service"></param>/
        public PermisosController(IPermisoService service)
        {
            _service = service;
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
    }
}
