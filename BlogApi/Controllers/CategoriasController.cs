using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _service;

    public CategoriasController(ICategoriaService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Administrador,Editor")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categorias = await _service.GetAllAsync();
        return Ok(categorias.Select(c => c.ToDto()));
    }

    [Authorize(Roles = "Administrador,Editor")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var categoria = await _service.GetByIdAsync(id);
        if (categoria == null)
            return NotFound();
        return Ok(categoria.ToDto());
    }

    [Authorize(Roles = "Administrador,Editor")]
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

    [Authorize(Roles = "Administrador,Editor")]
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

    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok)
            return NotFound();
        return NoContent();
    }
}
