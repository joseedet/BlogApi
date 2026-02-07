using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

/// <summary>
/// Controlador de EmailSettings
/// </summary>
[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Admin")] // Solo el admin puede gestionar esto
public class EmailSettingsController : ControllerBase
{
    private readonly IEmailSettingsService _service;
    private readonly IEmailService _emailService;

    /// <summary>
    /// Constructor de EmailSettings
    /// </summary>
    /// <param name="service"></param>
    /// <param name="emailService"></param>
    public EmailSettingsController(IEmailSettingsService service, IEmailService emailService)
    {
        _service = service;
        _emailService = emailService;
    }

    /// <summary>
    /// Obtiene las propiedades de email
    /// </summary>
    /// <returns>EmailSettingsDto</returns>
    // ------------------------------------------------------------
    // GET /api/emailsettings
    // ------------------------------------------------------------
    [Authorize(Policy = "Permiso:EmailSettings.Ver")]
    [HttpGet]
    public async Task<ActionResult<EmailSettingsDto>> Obtener()
    {
        var settings = await _service.ObtenerAsync();
        return Ok(settings);
    }

    /// <summary>
    /// Actualiza el email
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>EmailSettingsDto</returns>
    // ------------------------------------------------------------
    // PUT /api/emailsettings
    // ------------------------------------------------------------
    [Authorize(Policy = "Permiso:EmailSettings.Editar")]
    [HttpPut]
    public async Task<ActionResult<EmailSettingsDto>> Actualizar(
        [FromBody] EmailSettingsUpdateDto dto
    )
    {
        var actualizado = await _service.ActualizarAsync(dto);
        return Ok(actualizado);
    }

    /// <summary>
    /// Envía un email de prueba
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>IActionResult</returns>
    // ------------------------------------------------------------
    // POST /api/emailsettings/test
    // ------------------------------------------------------------
    [Authorize(Policy = "Permiso:EmailSettings.Test")]
    [HttpPost("test")]
    public async Task<IActionResult> EnviarEmailPrueba([FromBody] EmailTestRequest dto)
    {
        try
        {
            await _emailService.EnviarAsync(
                dto.Destinatario,
                "Email de prueba",
                "Este es un email de prueba."
            );
            return Ok(new { mensaje = "Email de prueba enviado correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(
                new { mensaje = "Error al enviar el email de prueba.", detalle = ex.Message }
            );
        }
    }
}
