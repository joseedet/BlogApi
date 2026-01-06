using BlogApi.Domain.Factories;
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
    private readonly INotificacionesService _notificaciones;

    public PostsController(IPostService service, INotificacionesService notificaciones)
    {
        _service = service;
        _notificaciones = notificaciones;
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

        // 🔥 Crear notificación usando la Factory del dominio 
        
        var notificacion = NotificacionFactory.NuevoPost(usuarioId: created.UsuarioId, postId: created.Id, titulo: created.Titulo); 
        // 🔥 Guardar y enviar por SignalR
        
         await _notificaciones.CrearAsync(notificacion);

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

    [HttpGet("buscar")]
    public async Task<IActionResult> Buscar([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Debe proporcionar un texto de búsqueda");

        var posts = await _service.SearchAsync(q);

        return Ok(posts.Select(p => p.ToDto()));
    }

    // Filtrar por categoría (ID)
    [HttpGet("categoria/{categoriaId:int}")]
    public async Task<IActionResult> GetByCategoria(int categoriaId)
    {
        var posts = await _service.GetByCategoriaAsync(categoriaId);
        return Ok(posts.Select(p => p.ToDto()));
    }

    // Filtrar por categoría (slug)
    [HttpGet("categoria/slug/{slug}")]
    public async Task<IActionResult> GetByCategoriaSlug(string slug)
    {
        var posts = await _service.GetByCategoriaSlugAsync(slug);
        return Ok(posts.Select(p => p.ToDto()));
    }

    // Filtrar por tag (ID)
    [HttpGet("tag/{tagId:int}")]
    public async Task<IActionResult> GetByTag(int tagId)
    {
        var posts = await _service.GetByTagAsync(tagId);
        return Ok(posts.Select(p => p.ToDto()));
    }

    // Filtrar por tag (nombre)
    [HttpGet("tag/nombre/{nombre}")]
    public async Task<IActionResult> GetByTagNombre(string nombre)
    {
        var posts = await _service.GetByTagNombreAsync(nombre);
        return Ok(posts.Select(p => p.ToDto()));
    }

    // Filtrar por autor (ID)
    [HttpGet("autor/{usuarioId:int}")]
    public async Task<IActionResult> GetByAutor(int usuarioId)
    {
        var posts = await _service.GetByAutorAsync(usuarioId);
        return Ok(posts.Select(p => p.ToDto()));
    }

    // Filtrar por autor (nombre)
    [HttpGet("autor/nombre/{nombre}")]
    public async Task<IActionResult> GetByAutorNombre(string nombre)
    {
        var posts = await _service.GetByAutorNombreAsync(nombre);
        return Ok(posts.Select(p => p.ToDto()));
    }

    [HttpGet("cursor")]
    public async Task<IActionResult> GetCursorPaged(int? after = null, int limit = 10)
    {
        var result = await _service.GetCursorPagedAsync(after, limit);

        return Ok(
            new CursorPaginationDto<PostDto>
            {
                Datos = result.Datos.Select(p => p.ToDto()),
                NextCursor = result.NextCursor,
            }
        );
    }
}
