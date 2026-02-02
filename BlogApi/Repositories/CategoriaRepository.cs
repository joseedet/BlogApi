using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Repositories.Interfaces;
using BlogApi.Utils.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repositories;

/// <summary>
/// Repositorio de categoria
/// </summary>
public class CategoriaRepository : GenericRepository<Categoria>, ICategoriaRepository
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context"></param>
    public CategoriaRepository(BlogDbContext context)
        : base(context) { }

    /// <summary>
    /// ¿Existe este Slug?
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    public Task<bool> SlugExistsAsync(string slug)
    {
        return _dbSet.AnyAsync(x => x.Slug == slug);
    }

    /// <summary>
    /// Contador de post por categoría
    /// </summary>
    /// <param name="categoriaId"></param>
    /// <returns>int</returns>
    public Task<int> CountPostsAsync(int categoriaId)
    {
        return _context.Posts.CountAsync(p =>
            p.CategoriaId == categoriaId && p.Estado == PostEstado.Publicado
        );
    }

    /// <summary>
    /// Cuenta el numero de post por categoría
    /// </summary>
    /// <returns>Entero</returns>
    public Task<int> CountAsync()
    {
        return _context.Categorias.CountAsync();
    }
}
