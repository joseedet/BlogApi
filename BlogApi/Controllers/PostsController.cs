using System.Security.Claims;
using BlogApi.Domain.Factories;
using BlogApi.DTO;
using BlogApi.Mapper;
using BlogApi.Models;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

/// <summary>
/// PostController
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _service;
    private readonly INotificacionesService _notificaciones;

    /// <summary>
    /// Constructor PostController
    /// </summary>
    /// <param name="service"></param>
    /// <param name="notificaciones"></param>
    public PostsController(IPostService service, INotificacionesService notificaciones)
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

    /// <summary>
    /// Obtener por Id solo panel administración
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    // ------------------------------------------------------------
    // GET BY ID
    // ------------------------------------------------------------
    // SOLO ADMIN/EDITOR/AUTOR
    [HttpGet("admin/{id}")]
    [Authorize(Roles = "Administrador,Editor,Autor")]
    public async Task<IActionResult> GetByIdAdmin(int id)
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
    /// <returns></returns>
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
                usuarioDestinoId: created.UsuarioId,
                // el dueño del post
                usuarioOrigenId: usuarioId,
                // el que lo creó
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
    /// Actualizar post
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Eliminar post
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Paginación
    /// </summary>
    /// <param name="pagina"></param>
    /// <param name="tamano"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Obtener post por slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Búsqueda
    /// </summary>
    /// <param name="q"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Obtención de post por categoría
    /// </summary>
    /// <param name="categoriaId"></param>
    /// <returns></returns>
    // ------------------------------------------------------------
    // BY CATEGORY
    // ------------------------------------------------------------
    [HttpGet("categoria/{categoriaId:int}")]
    public async Task<IActionResult> GetByCategoria(int categoriaId)
    {
        var posts = await _service.GetByCategoriaAsync(categoriaId);
        return Ok(posts.Select(p => p.ToDto()));
    }
    /// <summary>
    /// Categoia slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>

    [HttpGet("categoria/slug/{slug}")]
    public async Task<IActionResult> GetByCategoriaSlug(string slug)
    {
        var posts = await _service.GetByCategoriaSlugAsync(slug);
        return Ok(posts.Select(p => p.ToDto()));
    }

    /// <summary>
    /// Obtención de post por tag
    /// </summary>
    /// <param name="tagId"></param>
    /// <returns></returns>
    // ------------------------------------------------------------
    // BY TAG
    // ------------------------------------------------------------
    [HttpGet("tag/{tagId:int}")]
    public async Task<IActionResult> GetByTag(int tagId)
    {
        var posts = await _service.GetByTagAsync(tagId);
        return Ok(posts.Select(p => p.ToDto()));
    }

    /// <summary>
    /// Obtiene tag por nombre
    /// </summary>
    /// <param name="nombre"></param>
    /// <returns></returns>
    [HttpGet("tag/nombre/{nombre}")]
    public async Task<IActionResult> GetByTagNombre(string nombre)
    {
        var posts = await _service.GetByTagNombreAsync(nombre);
        return Ok(posts.Select(p => p.ToDto()));
    }

    /// <summary>
    /// /// Obtiene post por autor
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <returns></returns>
    // ------------------------------------------------------------
    // BY AUTHOR
    // ------------------------------------------------------------
    [HttpGet("autor/{usuarioId:int}")]
    public async Task<IActionResult> GetByAutor(int usuarioId)
    {
        var posts = await _service.GetByAutorAsync(usuarioId);
        return Ok(posts.Select(p => p.ToDto()));
    }

    /// <summary>
    /// Obtencion por nombre del autor
    /// </summary>
    /// <param name="nombre"></param>
    /// <returns></returns>
    [HttpGet("autor/nombre/{nombre}")]
    public async Task<IActionResult> GetByAutorNombre(string nombre)
    {
        var posts = await _service.GetByAutorNombreAsync(nombre);
        return Ok(posts.Select(p => p.ToDto()));
    }

    /// <summary>
    /// Paginación por cursor
    /// </summary>
    /// <param name="after"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Obtención de post por id parte pública
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    // PÚBLICO
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetById(int id)
    {
        var post = await _service.GetByIdAsync(id);
        if (post == null)
            return NotFound();

        await _service.IncrementViewCountAsync(id);

        return Ok(post.ToDto());
    }

    /// <summary>
    /// Obtiene los mas vistos
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    [HttpGet("most-viewed")]
    public async Task<ActionResult<List<PostDto>>> GetMostViewed([FromQuery] int count = 5)
    {
        var posts = await _service.GetMostViewedAsync(count);
        return Ok(posts);
    }

    /// <summary>
    /// Obtiene los más comentados
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    [HttpGet("most-commented")]
    public async Task<ActionResult<List<PostDto>>> GetMostCommented([FromQuery] int count = 5)
    {
        var posts = await _service.GetMostCommentedAsync(count);
        return Ok(posts);
    }

    /// <summary>
    /// Obtiene los relacionados
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    [HttpGet("{id:int}/related")]
    public async Task<ActionResult<List<PostDto>>> GetRelated(int id, [FromQuery] int count = 4)
    {
        var posts = await _service.GetRelatedPostsAsync(id, count);
        return Ok(posts);
    }
    /// <summary>
    /// Búsqueda avanzada
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    [HttpGet("search-advanced")]
    public async Task<IActionResult> SearchAdvanced([FromQuery] PostSearchParams p)
    {
        var posts = await _service.SearchAdvancedAsync(p);
        return Ok(posts.Select(x => x.ToDto()));
    }
}
