using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

/// <summary>
/// Controlador del Banner
/// </summary>
[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Admin")] // Solo admin gestiona banners
public class BannerController : ControllerBase
{
    private readonly IBannerService _service;

    /// <summary>
    /// Constructor del controlador
    /// </summary>
    /// <param name="service"></param>
    public BannerController(IBannerService service)
    {
        _service = service;
    }

    // ------------------------------------------------------------
    // GET /api/banner
    // ------------------------------------------------------------
    /// <summary>
    /// Obtener todos los banners
    /// </summary>
    /// <returns>IEnumerable BannerDto</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BannerDto>>> ObtenerTodos()
    {
        var banners = await _service.ObtenerTodosAsync();
        return Ok(banners);
    }

    // ------------------------------------------------------------
    // GET /api/banner/activos
    // ------------------------------------------------------------

    /// <summary>
    /// Obtener banners activos
    /// </summary>
    /// <returns>IEnumerable BannerDto</returns>
    [HttpGet("activos")]
    [AllowAnonymous] // Los banners activos se muestran en la web pública
    public async Task<ActionResult<IEnumerable<BannerDto>>> ObtenerActivos()
    {
        var banners = await _service.ObtenerActivosAsync();
        return Ok(banners);
    }

    // ------------------------------------------------------------
    // GET /api/banner/{id}
    // ------------------------------------------------------------

    /// <summary>
    /// Obtener por Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>BannerDto</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BannerDto>> ObtenerPorId(int id)
    {
        var banner = await _service.ObtenerPorIdAsync(id);
        if (banner == null)
            return NotFound(new { mensaje = "Banner no encontrado" });

        return Ok(banner);
    }

    // ------------------------------------------------------------
    // POST /api/banner
    // ------------------------------------------------------------
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<BannerDto>> Crear([FromForm] BannerCreateDto dto)
    {
        var creado = await _service.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
    }

    // ------------------------------------------------------------
    // PUT /api/banner/{id}
    // ------------------------------------------------------------

    /// <summary>
    /// Actualizar
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns>BannerDto</returns>
    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<BannerDto>> Actualizar(int id, [FromForm] BannerUpdateDto dto)
    {
        var actualizado = await _service.ActualizarAsync(id, dto);

        if (actualizado == null)
            return NotFound(new { mensaje = "Banner no encontrado" });

        return Ok(actualizado);
    }

    // ------------------------------------------------------------
    // DELETE /api/banner/{id}
    // ------------------------------------------------------------

    /// <summary>
    /// Elimina un banner
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Verdadero si se ha eliminado en caso contrario falso</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var eliminado = await _service.EliminarAsync(id);

        if (!eliminado)
            return NotFound(new { mensaje = "Banner no encontrado" });

        return NoContent();
    }
}
