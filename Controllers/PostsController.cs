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

    [Authorize(Policy = "PuedeEditarPost")]
    [HttpPut("{id}")]
    //[Authorize(Roles = "Administrador,Editor,Autor")]
    public async Task<IActionResult> Update(int id, CreatePostDto dto, bool puedeEditarTodo)
    {
        var post = new Post
        {
            Titulo = dto.Titulo,
            Contenido = dto.Contenido,
            CategoriaId = dto.CategoriaId,
            UsuarioId = dto.UsuarioId,
        };
        bool esAdmin = User.IsInRole("Administrador");
        bool esEditor = User.IsInRole("Editor");
        var ok = await _service.UpdateAsync(id, post, dto.TagIds, esAdmin || esEditor);
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

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(int pagina = 1, int tamano = 10)
    {
        var result = await _service.GetPagedAsync(pagina, tamano);
        return Ok(
            new PaginationDto<PostDto>
            {
                Pagina = result.Pagina,
                Tamano = result.Tamano,
                Total = result.Total,
                Datos = result.Datos.Select(p => p.ToDto()),
            }
        );
    }
    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var post = await _service.GetBySlugAsync(slug);
        if (post == null)
            return NotFound();

        return Ok(post.ToDto());
    }
}
