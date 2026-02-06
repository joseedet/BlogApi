using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    /// <summary>
    /// Controlador de NotificationSettings
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")] // Solo administradores
    public class NotificationSettingsController : ControllerBase
    {
        private readonly INotificationSettingsService _service;

        /// <summary>
        /// /Constructor de NotificationSettingsController
        /// </summary>
        /// <param name="service"></param>
        public NotificationSettingsController(INotificationSettingsService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtiene la configuración global de notificaciones.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<NotificationSettingsDto>> Get()
        {
            var settings = await _service.GetActiveAsync();
            if (settings == null)
                return NotFound("No hay configuración activa.");
            var dto = new NotificationSettingsDto
            {
                SendEmailOnComment = settings.SendEmailOnComment,
                SendEmailOnAdminMessage = settings.SendEmailOnAdminMessage,
                SendEmailOnSystemAlert = settings.SendEmailOnSystemAlert,
            };
            return Ok(dto);
        }

        /// <summary>
        ///  Actualiza la configuración global de notificaciones.
        ///  </summary>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] NotificationSettingsDto dto)
        {
            await _service.UpdateAsync(dto);
            return NoContent();
        }
    }
}
