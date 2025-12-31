using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _service;

    public PostsController(IPostService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "Administrador,Editor,Autor")]
    public async Task<IActionResult> GetAll()
    {
        var posts = await _service.GetAllAsync();
        return Ok(posts.Select(p => p.ToDto()));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador,Editor,Autor")]
    public async Task<IActionResult> GetById(int id)
    {
        var post = await _service.GetByIdAsync(id);
        if (post == null)
            return NotFound();

        return Ok(post.ToDto());
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Editor,Autor")]
    public async Task<IActionResult> Create(CreatePostDto dto)
    {
        var post = new Post
        {
            Titulo = dto.Titulo,
            Contenido = dto.Contenido,
            CategoriaId = dto.CategoriaId,
            UsuarioId = dto.UsuarioId,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow,
        };

        var created = await _service.CreateAsync(post, dto.TagIds);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador,Editor,Autor")]
    public async Task<IActionResult> Update(int id, CreatePostDto dto)
    {
        var post = new Post
        {
            Titulo = dto.Titulo,
            Contenido = dto.Contenido,
            CategoriaId = dto.CategoriaId,
            UsuarioId = dto.UsuarioId,
        };

        var ok = await _service.UpdateAsync(id, post, dto.TagIds);
        if (!ok)
            return NotFound();

        var updated = await _service.GetByIdAsync(id);
        return Ok(updated!.ToDto());
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador,Editor,Autor")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok)
            return NotFound();

        return NoContent();
    }
}
