using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    /// <summary>
    /// Controlador de estadísticas
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly IStatsService _statsService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="statsService"></param>
        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }

        /// <summary>
        /// Obtener estadísticas
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _statsService.GetEstadisticasAsync();
            return Ok(stats);
        }

        /// <summary>
        /// Actividad reciente
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet("actividad-reciente")]
        public async Task<IActionResult> GetActividadReciente([FromQuery] int limit = 10)
        {
            var actividad = await _statsService.GetActividadRecienteAsync(limit);
            return Ok(actividad);
        }
    }
}
