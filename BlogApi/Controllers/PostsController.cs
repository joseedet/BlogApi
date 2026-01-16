using System.Security.Claims;
using BlogApi.Domain.Factories;
using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _service;
    private readonly INotificacionService _notificaciones;

    public PostsController(IPostService service, INotificacionService notificaciones)
    {
        _service = service;
        _notificaciones = notificaciones;
    }

    // ------------------------------------------------------------
    // GET ALL
    // ------------------------------------------------------------
    [HttpGet]
    [Authorize(Roles = "Administrador,Editor,Autor")]
    public async Task<IActionResult> GetAll()
    {
        var posts = await _service.GetAllAsync();
        return Ok(posts.Select(p => p.ToDto()));
    }

    // ------------------------------------------------------------
    // GET BY ID
    // ------------------------------------------------------------
    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador,Editor,Autor")]
    public async Task<IActionResult> GetById(int id)
    {
        var post = await _service.GetByIdAsync(id);
        if (post == null)
            return NotFound();

        return Ok(post.ToDto());
    }

    // ------------------------------------------------------------
    // CREATE
    // ------------------------------------------------------------
    [HttpPost]
    [Authorize(Roles = "Administrador,Editor,Autor")]
    public async Task<IActionResult> Create(CreatePostDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            int usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var post = new Post
            {
                Titulo = dto.Titulo,
                Contenido = dto.Contenido,
                CategoriaId = dto.CategoriaId,
                UsuarioId = usuarioId,
            };

            var created = await _service.CreateAsync(post, dto.TagIds, usuarioId);

            if (created == null)
                return BadRequest("No se pudo crear el post");

            var notificacion = NotificacionFactory.NuevoPost(
                usuarioId: created.UsuarioId,
                postId: created.Id,
                titulo: created.Titulo
            );

            await _notificaciones.CrearAsync(notificacion);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ------------------------------------------------------------
    // UPDATE
    // ------------------------------------------------------------
    [HttpPut("{id}")]
    [Authorize(Policy = "PuedeEditarPost")]
    public async Task<IActionResult> Update(int id, CreatePostDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            int usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            bool esAdmin = User.IsInRole("Administrador");
            bool esEditor = User.IsInRole("Editor");

            var post = new Post
            {
                Titulo = dto.Titulo,
                Contenido = dto.Contenido,
                CategoriaId = dto.CategoriaId,
                UsuarioId = usuarioId,
            };

            var ok = await _service.UpdateAsync(
                id,
                post,
                dto.TagIds ?? new List<int>(),
                usuarioId,
                esAdmin || esEditor
            );

            if (!ok)
                return NotFound();

            var updated = await _service.GetByIdAsync(id);
            return Ok(updated!.ToDto());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ------------------------------------------------------------
    // DELETE
    // ------------------------------------------------------------
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador,Editor,Autor")]
    public async Task<IActionResult> Delete(int id)
    {
        int usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        bool esAdmin = User.IsInRole("Administrador");
        bool esEditor = User.IsInRole("Editor");
        var ok = await _service.DeleteAsync(id, usuarioId, esAdmin || esEditor);
        if (!ok)
            return NotFound();
        return NoContent();
    }

    // ------------------------------------------------------------
    // PAGED
    // ------------------------------------------------------------
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
                Items = result.Items.Select(p => p.ToDto()),
            }
        );
    }

    // ------------------------------------------------------------
    // GET BY SLUG
    // ------------------------------------------------------------
    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var post = await _service.GetBySlugAsync(slug);
        if (post == null)
            return NotFound();

        return Ok(post.ToDto());
    }

    // ------------------------------------------------------------
    // SEARCH
    // ------------------------------------------------------------
    [HttpGet("buscar")]
    public async Task<IActionResult> Buscar([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Debe proporcionar un texto de búsqueda");

        var posts = await _service.SearchAsync(q);
        return Ok(posts.Select(p => p.ToDto()));
    }

    // ------------------------------------------------------------
    // BY CATEGORY
    // ------------------------------------------------------------
    [HttpGet("categoria/{categoriaId:int}")]
    public async Task<IActionResult> GetByCategoria(int categoriaId)
    {
        var posts = await _service.GetByCategoriaAsync(categoriaId);
        return Ok(posts.Select(p => p.ToDto()));
    }

    [HttpGet("categoria/slug/{slug}")]
    public async Task<IActionResult> GetByCategoriaSlug(string slug)
    {
        var posts = await _service.GetByCategoriaSlugAsync(slug);
        return Ok(posts.Select(p => p.ToDto()));
    }

    // ------------------------------------------------------------
    // BY TAG
    // ------------------------------------------------------------
    [HttpGet("tag/{tagId:int}")]
    public async Task<IActionResult> GetByTag(int tagId)
    {
        var posts = await _service.GetByTagAsync(tagId);
        return Ok(posts.Select(p => p.ToDto()));
    }

    [HttpGet("tag/nombre/{nombre}")]
    public async Task<IActionResult> GetByTagNombre(string nombre)
    {
        var posts = await _service.GetByTagNombreAsync(nombre);
        return Ok(posts.Select(p => p.ToDto()));
    }

    // ------------------------------------------------------------
    // BY AUTHOR
    // ------------------------------------------------------------
    [HttpGet("autor/{usuarioId:int}")]
    public async Task<IActionResult> GetByAutor(int usuarioId)
    {
        var posts = await _service.GetByAutorAsync(usuarioId);
        return Ok(posts.Select(p => p.ToDto()));
    }

    [HttpGet("autor/nombre/{nombre}")]
    public async Task<IActionResult> GetByAutorNombre(string nombre)
    {
        var posts = await _service.GetByAutorNombreAsync(nombre);
        return Ok(posts.Select(p => p.ToDto()));
    }

    // ------------------------------------------------------------
    // CURSOR PAGINATION
    // ------------------------------------------------------------
    [HttpGet("cursor")]
    public async Task<IActionResult> GetCursorPaged(int? after = null, int limit = 10)
    {
        var result = await _service.GetCursorPagedAsync(after, limit);

        return Ok(
            new CursorPaginationDto<PostDto>
            {
                Items = result.Items.Select(p => p.ToDto()),
                NextCursor = result.NextCursor,
            }
        );
    }
}
