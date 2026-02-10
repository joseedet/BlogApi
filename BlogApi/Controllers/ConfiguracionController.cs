using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    /// <summary>
    /// Controlador para gestionar la configuración de la aplicación, incluyendo la configuración de caché.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ConfiguracionController : ControllerBase
    {
        private readonly ICacheConfigService _cacheConfigService;
        /// <summary>
        /// Constructor del controlador de configuración, inyectando el servicio de configuración de caché.
        /// </summary>
        /// <param name="cacheConfigService"></param>
        public ConfiguracionController(ICacheConfigService cacheConfigService)
        {
            _cacheConfigService = cacheConfigService;
        }
        /// <summary>
        /// Obtiene la configuración de caché actual de la aplicación. Requiere el permiso "Configuracion.Ver".
        /// </summary>
        [Authorize(Policy = "Permiso:Configuracion.Ver")]
        [HttpGet("cache-config")]
        public async Task<ActionResult<CacheConfigDto>> ObtenerCacheConfig()
        {
            var config = await _cacheConfigService.ObtenerConfigAsync();
            var dto = new CacheConfigDto
            {
                ExpiracionPostsSegundos = config.ExpiracionPostsSegundos,
                ExpiracionComentariosSegundos = config.ExpiracionComentariosSegundos,
                ExpiracionDashboardSegundos = config.ExpiracionDashboardSegundos,
                ExpiracionRolesSegundos = config.ExpiracionRolesSegundos,
                ExpiracionPermisosSegundos = config.ExpiracionPermisosSegundos,
                ExpiracionUsuariosSegundos = config.ExpiracionUsuariosSegundos,
            };
            return Ok(dto);
        }
        /// <summary>
        /// Actualiza la configuración de caché de la aplicación. Requiere el permiso "Configuracion.Editar".
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Policy = "Permiso:Configuracion.Editar")]
        [HttpPut("cache-config")]
        public async Task<IActionResult> ActualizarCacheConfig([FromBody] CacheConfigDto dto)
        {
            await _cacheConfigService.ActualizarConfigAsync(dto);
            return Ok("Configuración de caché actualizada correctamente.");
        }
    }
}
