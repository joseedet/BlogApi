using System;
using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services.Interfaces;

namespace BlogApi.Services;
/// <summary>
/// Implementacion de IPageService
/// </summary>
public class PageService:IPageService
{
    private readonly IPageRepository _pageRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<PageService> _logger;

/// <summary>
/// Constructor
/// </summary>
/// <param name="pageRepository"></param>
/// <param name="httpContextAccessor"></param>
/// <param name="logger"></param>
    public PageService(
        IPageRepository pageRepository,
        IHttpContextAccessor httpContextAccessor,
        ILogger<PageService> logger
    )
    {
        _pageRepository = pageRepository;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }
    // --------------------------------------------------------- // Crear página // ---------------------------------------------------------

    /// <summary>
    /// Crea una Page
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>PageDto</returns>
    /// <exception cref="ArgumentException"></exception>

    public async Task<PageDto> CrearAsync(CrearPageDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Titulo)) throw new ArgumentException("El título es obligatorio");
        var slug = await GenerarSlugUnicoAsync(dto.Titulo);
        var ip = _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        var ua = _httpContextAccessor?.HttpContext?.Request?.Headers["User-Agent"].ToString();
        var page = new Page
        {
            Titulo = dto.Titulo,
            Slug = slug,
            Contenido = dto.Contenido,
            Publicado = dto.Publicado,
            Creado = DateTime.UtcNow,
            Actualizado = DateTime.UtcNow,
            IpCreacion = ip,
            UserAgentCreacion = ua
        };
        if (dto.EsInicio)
        {
            var todas = await _pageRepository.ObtenerTodasAsync();
            foreach (var p in todas.Where(x => x.EsInicio))
            {

                p.EsInicio = false;
                await _pageRepository.ActualizarAsync(p);
                //await _pageRepository.CrearAsync(page);
            }
        }
        _logger.LogInformation("Página creada: {Titulo} (Slug={Slug})",
         page.Titulo, page.Slug);
        return MapToDto(page);
    }
    // --------------------------------------------------------- // Actualizar página // --------------------------------------------------------- 
    /// <summary>
    /// Actualiza Page
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns>PageDto</returns>
    /// <exception cref="KeyNotFoundException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public async Task<PageDto> ActualizarAsync(int id, ActualizarPageDto dto)
    {   
        var page = await _pageRepository.ObtenerPorIdAsync(id);
        if (page == null) throw new KeyNotFoundException("Página no encontrada");
        if (string.IsNullOrWhiteSpace(dto.Titulo)) throw new ArgumentException("El título es obligatorio");
        // Regenerar slug si cambia el título
        if (!string.Equals(page.Titulo, dto.Titulo, StringComparison.OrdinalIgnoreCase))
        {
            page.Slug = await GenerarSlugUnicoAsync(dto.Titulo, page.Id);
        }
        var ip = _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        var ua = _httpContextAccessor?.HttpContext?.Request?.Headers["User-Agent"].ToString();
        page.Titulo = dto.Titulo; page.Contenido = dto.Contenido;
        page.Publicado = dto.Publicado; page.Actualizado = DateTime.UtcNow;
        page.IpActualizacion = ip; page.UserAgentActualizacion = ua;
        if (dto.EsInicio)
        {
            var todas = await _pageRepository.ObtenerTodasAsync();
            foreach (var p in todas.Where(x => x.EsInicio))
            {
                p.EsInicio = false;
                await _pageRepository.ActualizarAsync(p);
            }
        }
        //await _pageRepository.ActualizarAsync(page);
        _logger.LogInformation("Página actualizada: {Titulo} (Slug={Slug})", page.Titulo, page.Slug);
        return MapToDto(page);
    }
    // --------------------------------------------------------- // Obtener por ID // ---------------------------------------------------------
    /// <summary>
    /// Obtiene página por Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>PageDto</returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task<PageDto> ObtenerPorIdAsync(int id)
    {
        var page = await _pageRepository.ObtenerPorIdAsync(id);
        if (page == null) throw new KeyNotFoundException("Página no encontrada");
        return MapToDto(page);
    }
    // --------------------------------------------------------- // Obtener por Slug // --------------------------------------------------------- 
    /// <summary>
    /// Obtene página por slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns>PageDto</returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task<PageDto> ObtenerPorSlugAsync(string slug)
    {
        var page = await _pageRepository.ObtenerPorSlugAsync(slug);
        if (page == null) throw new KeyNotFoundException("Página no encontrada");
        return MapToDto(page);
    }
    // --------------------------------------------------------- // Listado // ---------------------------------------------------------
    /// <summary>
    /// Obtiene todas páginas
    /// </summary>
    /// <returns>List&lt;ListadoDto&gt;</returns>

    public async Task<List<PageListadoDto>> ObtenerTodasAsync()
    {
        var pages = await _pageRepository.ObtenerTodasAsync();
        return pages.Select(p => new PageListadoDto
        {
            Id = p.Id,
            Titulo = p.Titulo,
            Slug = p.Slug,
            Publicado = p.Publicado,
            Actualizado = p.Actualizado
        }).ToList();
    }
    // --------------------------------------------------------- // Eliminar // ---------------------------------------------------------
    /// <summary>
    /// Elimina una página.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task EliminarAsync(int id)
    {
        var page = await _pageRepository.ObtenerPorIdAsync(id);
        if (page == null) throw new KeyNotFoundException("Página no encontrada");
        await _pageRepository.EliminarAsync(page); _logger.LogInformation("Página eliminada: {Titulo} (Slug={Slug})", page.Titulo, page.Slug);
    }
    // --------------------------------------------------------- // Helpers // ---------------------------------------------------------
    private async Task<string> GenerarSlugUnicoAsync(string titulo, int? idActual = null)
    {
        string baseSlug = CrearSlug(titulo);
        string slug = baseSlug; int contador = 1;
        while (true)
        {
            var existente = await _pageRepository.ObtenerPorSlugAsync(slug);
            if (existente == null || existente.Id == idActual) return slug;
            slug = $"{baseSlug}-{contador}"; contador++;
        }
    }
    private string CrearSlug(string texto)
    {
        texto = texto.ToLowerInvariant().Trim();
        texto = texto.Replace(" ", "-");
        texto = System.Text.RegularExpressions.Regex.Replace(texto, @"[^a-z0-9\-]", ""); return texto;
    }
    private PageDto MapToDto(Page p)
    {
        return new PageDto
        {
            Id = p.Id,
            Titulo = p.Titulo,
            Slug = p.Slug,
            Contenido = p.Contenido,
            Publicado = p.Publicado,
            Creado = p.Creado,
            Actualizado = p.Actualizado
        };
    }

    /// <summary>
    /// Obtenemos página de inicio
    /// </summary>
    /// <returns>PageDto</returns>
    public async Task<PageDto> ObtenerPaginaInicioAsync()
    {
        var pages = await _pageRepository.ObtenerTodasAsync();
        var inicio = pages.FirstOrDefault(p => p.EsInicio && p.Publicado);

        if (inicio == null)
            throw new KeyNotFoundException("No hay página de inicio configurada");

        return MapToDto(inicio);
    }

    /// <summary>
    /// Restaura la versión de la página
    /// </summary>
    /// <param name="versionId"></param>
    /// <returns>PageDto</returns>
    public async Task<PageDto> RestaurarVersionAsync(int versionId)
    {
        var version = await _pageRepository.ObtenerVersionPorIdAsync(versionId);
        if (version == null)
            throw new KeyNotFoundException("Versión no encontrada");

        var page = await _pageRepository.ObtenerPorIdAsync(version.PageId);
        if (page == null)
            throw new KeyNotFoundException("Página no encontrada");

        page.Titulo = version.Titulo;
        page.Slug = version.Slug;
        page.Contenido = version.Contenido;
        page.Publicado = version.Publicado;
        page.EsInicio = version.EsInicio;
        page.Actualizado = DateTime.UtcNow;

        await _pageRepository.ActualizarAsync(page);

        return MapToDto(page);
    }
    /// <summary>
    /// Obtiene la versión de una página dada
    /// </summary>
    /// <param name="pageId"></param>
    /// <returns>List&lt;PageVersionDto&gt;</returns>
    public async Task<List<PageVersionDto>> ObtenerVersionesAsync(int pageId)
    {
        var versiones = await _pageRepository.ObtenerVersionesAsync(pageId);

        return versiones
            .Select(v => new PageVersionDto
            {
                Id = v.Id,
                PageId = v.PageId,
                Titulo = v.Titulo,
                Slug = v.Slug,
                Contenido = v.Contenido,
                Publicado = v.Publicado,
                EsInicio = v.EsInicio,
                FechaVersion = v.FechaVersion,
                IpCreacion = v.IpCreacion,
                UserAgentCreacion = v.UserAgentCreacion,
            })
            .ToList();
    }

    /// <summary>
    /// Obtiene verisión de página especifica
    /// </summary>
    /// <param name="versionId"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task<PageVersionDto> ObtenerVersionPorIdAsync(int versionId)
    {
        var version = await _pageRepository.ObtenerVersionPorIdAsync(versionId);

        if (version == null)
            throw new KeyNotFoundException("Versión no encontrada");

        return new PageVersionDto
        {
            Id = version.Id,
            PageId = version.PageId,
            Titulo = version.Titulo,
            Slug = version.Slug,
            Contenido = version.Contenido,
            Publicado = version.Publicado,
            EsInicio = version.EsInicio,
            FechaVersion = version.FechaVersion,
            IpCreacion = version.IpCreacion,
            UserAgentCreacion = version.UserAgentCreacion,
        };
    }
}
