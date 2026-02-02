using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly IStatsService _statsService;

        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _statsService.GetEstadisticasAsync();
            return Ok(stats);
        }

        [HttpGet("actividad-reciente")]
        public async Task<IActionResult> GetActividadReciente([FromQuery] int limit = 10)
        {
            var actividad = await _statsService.GetActividadRecienteAsync(limit);
            return Ok(actividad);
        }
    }
}
