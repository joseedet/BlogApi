using BlogApi.DTO;
using BlogApi.Mapper;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

/// <summary>
/// Controlador de API para manejar las operaciones relacionadas con los mensajes de contacto. Este controlador se encarga de recibir las solicitudes HTTP relacionadas con los mensajes de contacto, procesarlas y devolver las respuestas correspondientes. Actualmente, el controlador está vacío y no contiene ninguna acción o método, pero se espera que en el futuro se implementen las funcionalidades necesarias para gestionar los mensajes de contacto en la aplicación.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MensajeContactoController : ControllerBase
{
    private readonly IMensajeContactoService _service;

    /// <summary>
    /// Constructor del controlador de mensajes de contacto. Recibe una instancia del servicio de mensajes de contacto a través de la inyección de dependencias, lo que permite al controlador utilizar los métodos definidos en el servicio para gestionar los mensajes de contacto. Este constructor es esencial para establecer la conexión entre el controlador y el servicio, permitiendo que el controlador delegue las operaciones relacionadas con los mensajes de contacto al servicio correspondiente.
    /// </summary>
    /// <param name="service"></param>
    public MensajeContactoController(IMensajeContactoService service)
    {
        _service = service;
    }

    /// <summary>
    /// Crea un nuevo mensaje de contacto enviado desde el formulario.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MensajeContactoDto>> CrearMensaje(
        [FromBody] MensajeContactoCrearDto dto
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Capturar IP del cliente
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var modelo = dto.ToModel(ip);

        var creado = await _service.CrearMensajeAsync(modelo);

        return CreatedAtAction(nameof(ObtenerMensajePorId), new { id = creado.Id }, creado.ToDto());
    }

    /// <summary>
    /// Obtiene un mensaje de contacto por su ID.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MensajeContactoDto>> ObtenerMensajePorId(int id)
    {
        var mensaje = await _service.ObtenerMensajePorIdAsync(id);

        if (mensaje is null)
            return NotFound($"No existe un mensaje con el ID {id}");

        return Ok(mensaje.ToDto());
    }

    /// <summary>
    /// Obtiene todos los mensajes de contacto enviados.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MensajeContactoDto>>> ObtenerMensajes()
    {
        var mensajes = await _service.ObtenerMensajesAsync();

        return Ok(mensajes.Select(m => m.ToDto()));
    }
}
