using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor del controlador de configuración, inyectando el servicio de configuración de caché.
        /// </summary>
        /// <param name="cacheConfigService"></param>
        /// <param name="configuration"></param>
        public ConfiguracionController(
            ICacheConfigService cacheConfigService,
            IConfiguration configuration
        )
        {
            _cacheConfigService = cacheConfigService;
            _configuration = configuration;
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
                ExpiracionPostsListadoSegundos = config.ExpiracionPostsListadoSegundos,
                ExpiracionPostPorSlugSegundos = config.ExpiracionPostPorSlugSegundos,
                ExpiracionPostsPorCategoriaSlugSegundos =
                    config.ExpiracionPostsPorCategoriaSlugSegundos,
                ExpiracionPostsPorCategoriaIdSegundos =
                    config.ExpiracionPostsPorCategoriaIdSegundos,
                ExpiracionPostsPorTagIdSegundos = config.ExpiracionPostsPorTagIdSegundos,
                ExpiracionPostsPorTagNombreSegundos = config.ExpiracionPostsPorTagNombreSegundos,
            };
            return Ok(dto);
        }

        /// <summary>
        /// Actualiza la configuración de caché de la aplicación. Requiere el permiso "Configuracion.Editar".
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Policy = "Permiso:Cache.Editar")]
        [HttpPut("cache-config")]
        public async Task<IActionResult> ActualizarCacheConfig([FromBody] CacheConfigDto dto)
        {
            await _cacheConfigService.ActualizarConfigAsync(dto);
            return Ok("Configuración de caché actualizada correctamente.");
        }

        /// <summary>
        /// Obtención de la caché
        /// </summary>
        /// <returns></returns>
        [Authorize(Policy = "Permiso:Cache.Ver")]
        [HttpGet("cache")]
        public async Task<IActionResult> GetCacheConfig()
        {
            var config = await _cacheConfigService.ObtenerConfigAsync();

            return Ok(
                new
                {
                    ExpiracionPostsSegundos = config.ExpiracionPostsSegundos,
                    Proveedor = _configuration["Cache:Provider"],
                }
            );
        }
    }
}
