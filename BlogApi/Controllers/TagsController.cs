using BlogApi.DTO;
using BlogApi.Mapper;
using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;
/// <summary>
/// Controlador de Tags
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    /// <summary>
    /// Controlador de Tags que maneja las operaciones CRUD para las etiquetas de las publicaciones. Este controlador utiliza el servicio ITagService para interactuar con la capa de negocio y realizar las operaciones necesarias en la base de datos. Las acciones del controlador están protegidas por políticas de autorización específicas para cada operación, lo que garantiza que solo los usuarios con los permisos adecuados puedan crear, editar o eliminar etiquetas. El controlador también incluye métodos para obtener todas las etiquetas o una etiqueta específica por su ID, facilitando la gestión de las etiquetas en la aplicación.
    /// </summary>
    private readonly ITagService _service;

    /// <summary>
    /// Constructor del controlador de Tags, que recibe una instancia del servicio de Tags a través de inyección de dependencias. Este servicio se utiliza para realizar las operaciones CRUD en las etiquetas. El constructor asegura que el controlador tenga acceso a las funcionalidades necesarias para gestionar las etiquetas de manera eficiente y desacoplada de la implementación específica del servicio, lo que facilita la mantenibilidad y la testabilidad del código.
    /// </summary>     
    /// <param name="service"></param>
    public TagsController(ITagService service)
    {
        _service = service;
    }
    /// <summary>
    /// Obtiene todas las etiquetas disponibles en el sistema. Este método maneja las solicitudes GET a /api/tags y devuelve una lista de etiquetas en formato JSON. La acción está protegida por la política de autorización "Permiso:Tags.Ver", lo que significa que solo los usuarios con este permiso podrán acceder a esta información. El método utiliza el servicio ITagService para obtener los datos de las etiquetas de manera asíncrona, lo que permite manejar solicitudes de manera eficiente sin bloquear el hilo de ejecución. La respuesta incluye una lista de objetos TagDto, que representan las etiquetas con sus propiedades relevantes para el cliente.
    /// </summary>
    /// <returns></returns>
    [Authorize(Policy = "Permiso:Tags.Ver")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tags = await _service.GetAllAsync();
        return Ok(tags.Select(t => t.ToDto()));
    }
    /// <summary>
    /// Obtiene una etiqueta por su ID. Este método maneja las solicitudes GET a /api/tags/{id} y devuelve la etiqueta correspondiente al ID proporcionado en formato JSON. La acción está protegida por la política de autorización "Permiso:Tags.Ver", lo que significa que solo los usuarios con este permiso podrán acceder a esta información. El método utiliza el servicio ITagService para obtener los datos de la etiqueta de manera asíncrona, lo que permite manejar solicitudes de manera eficiente sin bloquear el hilo de ejecución. Si la etiqueta con el ID especificado no se encuentra, el método devuelve un resultado NotFound; de lo contrario, devuelve un resultado Ok con el objeto TagDto que representa la etiqueta encontrada.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Policy = "Permiso:Tags.Ver")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var tag = await _service.GetByIdAsync(id);
        if (tag == null)
            return NotFound();
        return Ok(tag.ToDto());
    }

    //[Authorize(Roles = "Administrador,Editor")]
    /// <summary>
    /// Crea una nueva etiqueta en el sistema. Este método maneja las solicitudes POST a /api/tags y recibe un objeto TagDto en el cuerpo de la solicitud, que contiene la información necesaria para crear la etiqueta. La acción está protegida por la política de autorización "Permiso:Tags.Crear", lo que significa que solo los usuarios con este permiso podrán realizar esta operación. El método utiliza el servicio ITagService para crear la etiqueta de manera asíncrona, lo que permite manejar solicitudes de manera eficiente sin bloquear el hilo de ejecución. Si la creación es exitosa, el método devuelve un resultado CreatedAtAction con la ubicación de la nueva etiqueta y su representación en formato JSON; de lo contrario, devuelve un resultado BadRequest con un mensaje de error.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [Authorize(Policy = "Permiso:Tags.Crear")]
    [HttpPost]
    public async Task<IActionResult> Create(TagDto dto)
    {
        var tag = new Tag { Nombre = dto.Nombre };
        var created = await _service.CreateAsync(tag);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
    }

    //[Authorize(Roles = "Administrador,Editor")]
    /// <summary>
    /// Actualiza una etiqueta existente por su ID. Este método maneja las solicitudes PUT a /api/tags/{id} y recibe un objeto TagDto en el cuerpo de la solicitud, que contiene la información actualizada de la etiqueta. La acción está protegida por la política de autorización "Permiso:Tags.Editar", lo que significa que solo los usuarios con este permiso podrán realizar esta operación. El método utiliza el servicio ITagService para actualizar la etiqueta de manera asíncrona, lo que permite manejar solicitudes de manera eficiente sin bloquear el hilo de ejecución. Si la actualización es exitosa, el método devuelve un resultado Ok con la representación actualizada de la etiqueta en formato JSON; si la etiqueta con el ID especificado no se encuentra, devuelve un resultado NotFound; de lo contrario, devuelve un resultado BadRequest con un mensaje de error.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [Authorize(Policy = "Permiso:Tags.Editar")]
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
    /// <summary>
    /// Elimina una etiqueta por su ID. Este método maneja las solicitudes DELETE a /api/tags/{id} y elimina la etiqueta correspondiente al ID proporcionado. La acción está protegida por la política de autorización "Permiso:Tags.Eliminar", lo que significa que solo los usuarios con este permiso podrán realizar esta operación. El método utiliza el servicio ITagService para eliminar la etiqueta de manera asíncrona, lo que permite manejar solicitudes de manera eficiente sin bloquear el hilo de ejecución. Si la eliminación es exitosa, el método devuelve un resultado NoContent; si la etiqueta con el ID especificado no se encuentra, devuelve un resultado NotFound; de lo contrario, devuelve un resultado BadRequest con un mensaje de error.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>

    //[Authorize(Roles = "Administrador,Editor")]
    [Authorize(Policy = "Permiso:Tags.Eliminar")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok)
            return NotFound();
        return NoContent();
    }
}
