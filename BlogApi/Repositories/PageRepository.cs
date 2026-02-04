using System;
using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repositories;

/// <summary>
/// Clase PageRepository
/// </summary>
public class PageRepository : IPageRepository
{
    private readonly BlogDbContext _context;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context"></param>
    public PageRepository(BlogDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtenemos pagina por Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Page</returns>
    public async Task<Page> ObtenerPorIdAsync(int id)
    {
        return await _context.Pages.FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Obtener pagina por slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns>Page</returns>
    public async Task<Page> ObtenerPorSlugAsync(string slug)
    {
        return await _context.Pages.FirstOrDefaultAsync(p => p.Slug == slug);
    }

    /// <summary>
    /// Obtener todas las páginas.
    /// </summary>
    /// <returns>List&lt;Page&gt;</returns>
    public async Task<List<Page>> ObtenerTodasAsync()
    {
        return await _context.Pages.OrderBy(p => p.Titulo).ToListAsync();
    }

    /// <summary>
    /// Crear página
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task CrearAsync(Page page)
    {
        _context.Pages.Add(page);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Actualizar página
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task ActualizarAsync(Page page)
    {
        // Guardar versión antes de modificar
        await this.GuardarVersionAsync(
            new PageVersion
            {
                PageId = page.Id,
                Titulo = page.Titulo,
                Slug = page.Slug,
                Contenido = page.Contenido,
                Publicado = page.Publicado,
                EsInicio = page.EsInicio,
                FechaVersion = DateTime.UtcNow,
                IpCreacion = page.IpActualizacion,
                UserAgentCreacion = page.UserAgentActualizacion,
            }
        );

        _context.Pages.Update(page);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Elimina página
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task EliminarAsync(Page page)
    {
        _context.Pages.Remove(page);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Guarda versión
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public async Task GuardarVersionAsync(PageVersion version)
    {
        _context.PageVersions.Add(version);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Obtiene versiónes de la pagina
    /// </summary>
    /// <param name="pageId"></param>
    /// <returns>List&lt;PageVersion&gt;</returns>
    public async Task<List<PageVersion>> ObtenerVersionesAsync(int pageId)
    {
        return await _context
            .PageVersions.Where(v => v.PageId == pageId)
            .OrderByDescending(v => v.FechaVersion)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene versión de la pagina
    /// </summary>
    /// <param name="versionId"></param>
    /// <returns>PageVersion</returns>
    public async Task<PageVersion> ObtenerVersionPorIdAsync(int versionId)
    {
        return await _context.PageVersions.FirstOrDefaultAsync(v => v.Id == versionId);
    }
}
