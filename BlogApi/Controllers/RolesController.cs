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

        /// <summary>
        /// Constructor que recibe el servicio de roles a través de inyección de dependencias
        /// </summary>
        /// <param name="service"></param>
        public RolesController(IRolService service)
        {
            _service = service;
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
    }
}
