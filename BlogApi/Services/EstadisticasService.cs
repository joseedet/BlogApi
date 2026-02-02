using System;
using BlogApi.DTO;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

/// <summary>
/// Estadísticas
/// </summary>
public class EstadisticasService : IStatsService
{
    /// <summary>
    /// Repositorio de post
    /// </summary>
    public readonly IPostRepository _postRepository;

    /// <summary>
    /// Repositorio de categoria.
    /// </summary>
    public readonly ICategoriaRepository _categoriaRepository;

    /// <summary>
    /// Repositorio de Tag.
    /// </summary>
    public readonly ITagRepository _tagRepository;

    /// <summary>
    /// Repositorio de usuario.
    /// </summary>
    public readonly IUsuarioRepository _usuarioRepository;

    /// <summary>
    /// Repositorio de comentario.
    /// </summary>
    public readonly IComentarioRepository _comentarioRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="postRepository"></param>
    /// <param name="categoriaRepository"></param>
    /// <param name="tagRepository"></param>
    /// <param name="usuarioRepository"></param>
    /// <param name="comentarioRepository"></param>
    public EstadisticasService(
        IPostRepository postRepository,
        ICategoriaRepository categoriaRepository,
        ITagRepository tagRepository,
        IUsuarioRepository usuarioRepository,
        IComentarioRepository comentarioRepository
    )
    {
        _postRepository = postRepository;
        _categoriaRepository = categoriaRepository;
        _tagRepository = tagRepository;
        _usuarioRepository = usuarioRepository;
        _comentarioRepository = comentarioRepository;
    }

    /// <summary>
    /// Obtiene las estadisticas.
    /// </summary>
    /// <returns></returns>
    public async Task<BlogStatsDto> GetEstadisticasAsync()
    {
        return new BlogStatsDto
        {
            TotalPosts = await _postRepository.CountAsync(),
            TotalCategorias = await _categoriaRepository.CountAsync(),
            TotalTags = await _tagRepository.CountAsync(),
            TotalUsuarios = await _usuarioRepository.CountAsync(),
            TotalComentarios = await _comentarioRepository.CountAsync(),
        };
    }

    /// <summary>
    /// Actividad reciente
    /// </summary>
    /// <param name="limit"></param>
    /// <returns>ActividadRecienteDto</returns>
    public async Task<ActividadRecienteDto> GetActividadRecienteAsync(int limit = 10)
    {
        return new ActividadRecienteDto
        {
            UltimosPosts = await _postRepository.GetRecentPostsAsync(limit),
            UltimosComentarios = await _comentarioRepository.GetRecentComentariosAsync(limit),
            UltimosUsuarios = await _usuarioRepository.GetRecentUsuariosAsync(limit),
        };
    }
}
