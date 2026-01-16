using BlogApi.DTO;
using BlogApi.Mapper;
using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ITagService _service;

    public TagsController(ITagService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tags = await _service.GetAllAsync();
        return Ok(tags.Select(t => t.ToDto()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var tag = await _service.GetByIdAsync(id);
        if (tag == null)
            return NotFound();
        return Ok(tag.ToDto());
    }

    [Authorize(Roles = "Administrador,Editor")]
    [HttpPost]
    public async Task<IActionResult> Create(TagDto dto)
    {
        var tag = new Tag { Nombre = dto.Nombre };
        var created = await _service.CreateAsync(tag);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
    }

    [Authorize(Roles = "Administrador,Editor")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TagDto dto)
    {
        var tag = new Tag { Nombre = dto.Nombre };
        var ok = await _service.UpdateAsync(id, tag);
        if (!ok)
            return NotFound();
        var updated = await _service.GetByIdAsync(id);
        return Ok(updated!.ToDto());
    }

    [Authorize(Roles = "Administrador,Editor")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok)
            return NotFound();
        return NoContent();
    }
}
