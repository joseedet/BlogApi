using BlogApi.DTO;
using BlogApi.Mapper;
using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;
/// <summary>
/// Controlador para gestionar categorías en el blog, permite crear, actualizar, eliminar y obtener categorías.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _service;
    /// <summary>
    /// Constructor del controlador de categorías, inyecta el servicio necesario para gestionar categorías.
    /// </summary>
    /// <param name="service"></param>
    public CategoriasController(ICategoriaService service)
    {
        _service = service;
    }

    //[Authorize(Roles = "Administrador,Editor")]
    /// <summary>
    /// Obtiene todas las categorías del blog.
    /// </summary>
    /// <returns></returns>
    [Authorize(Policy = "Permiso:Categorias.Ver")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categorias = await _service.GetAllAsync();
        return Ok(categorias.Select(c => c.ToDto()));
    }

    //[Authorize(Roles = "Administrador,Editor")]
    /// <summary>
    /// Obtiene una categoría por su ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Policy = "Permiso:Categorias.Ver")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var categoria = await _service.GetByIdAsync(id);
        if (categoria == null)
            return NotFound();
        return Ok(categoria.ToDto());
    }

    //[Authorize(Roles = "Administrador,Editor")]
    /// <summary>   
    /// Crea una nueva categoría.
    /// </summary>
    [Authorize(Policy = "Permiso:Categorias.Crear")]
    [HttpPost]
    public async Task<IActionResult> Create(CategoriaDto categoria)
    {
        var cat = new Categoria { Nombre = categoria.Nombre };
        var created = await _service.CreateAsync(cat);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
    }

    /*public async Task<IActionResult> Create(Categoria categoria)
    {
        var created = await _service.CreateAsync(categoria);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }*/

    /// <summary>
    /// Actualiza una categoría existente por su ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="categoria"></param>
    /// <returns></returns>
    /// </summary>

    //[Authorize(Roles = "Administrador,Editor")]
    [Authorize(Policy = "Permiso:Categorias.Editar")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CategoriaDto dto)
    {
        var categoria = new Categoria { Nombre = dto.Nombre };
        var ok = await _service.UpdateAsync(id, categoria);
        if (!ok)
            return NotFound();
        return NoContent();
    }

    /*public async Task<IActionResult> Update(int id, Categoria categoria)
    {
        var ok = await _service.UpdateAsync(id, categoria);
        if (!ok)
            return NotFound();
        return NoContent();
    }*/

    /// <summary>
    /// Elimina una categoría por su ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    //[Authorize(Roles = "Administrador")]
    [Authorize(Policy = "Permiso:Categorias.Eliminar")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok)
            return NotFound();
        return NoContent();
    }
}
