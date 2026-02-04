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

    public async Task CrearAsync(Page page)
    {
        _context.Pages.Add(page);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Page page)
    {
        _context.Pages.Update(page);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Page page)
    {
        _context.Pages.Remove(page);
        await _context.SaveChangesAsync();
    }
}
