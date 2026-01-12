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
    private readonly INotificacionesService _notificaciones;

    /// <summary>
    /// Constructor de PostsController
    /// </summary>
    /// <param name="service"></param>
    /// <param name="notificaciones"></param>
    public PostsController(IPostService service, INotificacionesService notificaciones)
    {
        _service = service;
        _notificaciones = notificaciones;
    }

    /// <summary>
    /// Obtiene todos los posts
    /// </summary>
    /// <returns>Todos los posts</returns>
    [HttpGet]
    [Authorize(Roles = "Administrador,Editor,Autor")]
    public async Task<IActionResult> GetAll()
    {
        var posts = await _service.GetAllAsync();
        return Ok(posts.Select(p => p.ToDto()));
    }

    /// <summary>
    /// Obtiene un post por su ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Post</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador,Editor,Autor")]
    public async Task<IActionResult> GetById(int id)
    {
        var post = await _service.GetByIdAsync(id);
        if (post == null)
            return NotFound();

        return Ok(post.ToDto());
    }

    /// <summary>
    /// Crea un nuevo post
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>Post</returns>
    [HttpPost]
    [Authorize(Roles = "Administrador,Editor,Autor")]
    public async Task<IActionResult> Create(CreatePostDto dto)
    {
        try
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

    /// <summary>
    /// Actualiza un post existente
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns>Post actualizado</returns>
    [Authorize(Policy = "PuedeEditarPost")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreatePostDto dto)
    {
        try
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
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un post por su ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Notificación de eliminación</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador,Editor,Autor")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Obtiene posts paginados
    /// </summary>
    /// <param name="pagina"></param>
    /// <param name="tamano"></param>
    /// <returns>Posts paginados</returns>
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

    /// <summary>
    /// Obtiene un post por su slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns>Post</returns>
    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var post = await _service.GetBySlugAsync(slug);
        if (post == null)
            return NotFound();

        return Ok(post.ToDto());
    }

    /// <summary>
    /// Busca posts por texto
    /// </summary>
    /// <param name="q"></param>
    /// <returns>Posts encontrados</returns>
    [HttpGet("buscar")]
    public async Task<IActionResult> Buscar([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Debe proporcionar un texto de búsqueda");

        var posts = await _service.SearchAsync(q);

        return Ok(posts.Select(p => p.ToDto()));
    }

    /// <summary>
    /// Obtiene posts por categoría ID
    /// </summary>
    /// <param name="categoriaId"></param>
    /// <returns>Posts en la categoría</returns>
    [HttpGet("categoria/{categoriaId:int}")]
    public async Task<IActionResult> GetByCategoria(int categoriaId)
    {
        var posts = await _service.GetByCategoriaAsync(categoriaId);
        return Ok(posts.Select(p => p.ToDto()));
    }

    /// <summary>
    /// Obtiene posts por slug de categoría
    /// </summary>
    /// <param name="slug"></param>
    /// <returns>Posts en la categoría</returns>
    [HttpGet("categoria/slug/{slug}")]
    public async Task<IActionResult> GetByCategoriaSlug(string slug)
    {
        var posts = await _service.GetByCategoriaSlugAsync(slug);
        return Ok(posts.Select(p => p.ToDto()));
    }

    /// <summary>
    /// Obtiene posts por tag ID
    /// </summary>
    /// <param name="tagId"></param>
    /// <returns>Posts con el tag</returns>
    [HttpGet("tag/{tagId:int}")]
    public async Task<IActionResult> GetByTag(int tagId)
    {
        var posts = await _service.GetByTagAsync(tagId);
        return Ok(posts.Select(p => p.ToDto()));
    }

    /// <summary>
    /// Obtiene posts por nombre de tag
    /// </summary>
    /// <param name="nombre"></param>
    /// <returns>Posts con el tag</returns>
    [HttpGet("tag/nombre/{nombre}")]
    public async Task<IActionResult> GetByTagNombre(string nombre)
    {
        var posts = await _service.GetByTagNombreAsync(nombre);
        return Ok(posts.Select(p => p.ToDto()));
    }

    /// <summary>
    /// Obtiene posts por autor ID
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns>Posts del autor</returns>
    [HttpGet("autor/{usuarioId:int}")]
    public async Task<IActionResult> GetByAutor(int usuarioId)
    {
        var posts = await _service.GetByAutorAsync(usuarioId);
        return Ok(posts.Select(p => p.ToDto()));
    }

    /// <summary>
    /// Obtiene posts por nombre de autor
    /// </summary>
    /// <param name="nombre"></param>
    /// <returns>Posts del autor</returns>
    [HttpGet("autor/nombre/{nombre}")]
    public async Task<IActionResult> GetByAutorNombre(string nombre)
    {
        var posts = await _service.GetByAutorNombreAsync(nombre);
        return Ok(posts.Select(p => p.ToDto()));
    }

    /// <summary>
    /// Obtiene posts con paginación por cursor
    /// </summary>
    /// <param name="after"></param>
    /// <param name="limit"></param>
    /// <returns>Posts paginados por cursor</returns>
    [HttpGet("cursor")]
    public async Task<IActionResult> GetCursorPaged(int? after = null, int limit = 10)
    {
        var result = await _service.GetCursorPagedAsync(after, limit);

        return Ok(
            new CursorPaginationDto<PostDto>
            {
                Items = result.Items.Select(p => p.ToDto()), // ✔ corregido
                NextCursor = result.NextCursor,
            }
        );
    }
}
