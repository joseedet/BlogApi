using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    /// <summary>
    /// Page Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly IPageService _pageService;
        private readonly ILogger<PageController> _logger;

        /// <summary>
        /// Constructor de PageController
        /// </summary>
        /// <param name="pageService"></param>
        /// <param name="logger"></param>
        public PageController(IPageService pageService, ILogger<PageController> logger)
        {
            _pageService = pageService;
            _logger = logger;
        }
        // --------------------------------------------------------- // Crear página //
        // POST: api/page // ---------------------------------------------------------
        /// <summary>
        /// Crear página
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearPageDto dto)
        {
            var result = await _pageService.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = result.Id }, result);
        }
        // --------------------------------------------------------- // Actualizar página
        // PUT: api/page/{id} // ---------------------------------------------------------
        /// <summary>
        /// Actualizar página
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarPageDto dto)
        {
            var result = await _pageService.ActualizarAsync(id, dto);
            return Ok(result);
        }
        // --------------------------------------------------------- // Obtener página por ID 
        // GET: api/page/{id} // ---------------------------------------------------------
        /// <summary>
        /// Obtener página por ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var result = await _pageService.ObtenerPorIdAsync(id);
            return Ok(result);
        } // --------------------------------------------------------- // Obtener página por slug
        // GET: api/page/slug/{slug} // ---------------------------------------------------------
        /// <summary>
        /// Obtener página por slug
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> ObtenerPorSlug(string slug)
        {
            var result = await _pageService.ObtenerPorSlugAsync(slug);
            return Ok(result);
        }
        // --------------------------------------------------------- // Listado de páginas 
        // GET: api/page // ---------------------------------------------------------
        /// <summary>
        /// Listado de páginas
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            var result = await _pageService.ObtenerTodasAsync();
            return Ok(result);
        }
     // --------------------------------------------------------- // Eliminar página
     // DELETE: api/page/{id} // --------------------------------------------------------- 
/// <summary>
/// Eliminar página
/// </summary>
/// <param name="id"></param>
/// <returns></returns>
     
     [HttpDelete("{id:int}")] public async Task<IActionResult> Eliminar(int id)
        {
            await _pageService.EliminarAsync(id); return NoContent();
         }
    }
}
