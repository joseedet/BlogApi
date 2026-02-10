using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    /// <summary>
    /// Controlador para el dashboard, que proporciona una vista general de las estadísticas del sistema, incluyendo el número total de usuarios, roles, permisos y la actividad reciente.
    /// </summary>
    [Authorize(Policy = "Permiso:Usuarios.Ver")]
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        /// <summary>
        /// Constructor del controlador de dashboard, que recibe una instancia del servicio de dashboard a través de inyección de dependencias.
        /// </summary>
        /// <param name="dashboardService"></param>
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Obtiene los datos del dashboard, incluyendo estadísticas de usuarios, roles, permisos y actividad reciente. Requiere el permiso "Permiso:Usuarios.Ver".
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ObtenerDashboard()
        {
            var dashboard = await _dashboardService.ObtenerDashboardAsync();
            return Ok(dashboard);
        }
    }
}
