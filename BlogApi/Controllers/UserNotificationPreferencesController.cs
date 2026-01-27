using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    /// <summary>
    /// Controlador de UserNotificationPreference
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserNotificationPreferencesController : ControllerBase
    {
        private readonly IUserNotificationPreferencesService _service;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service"></param>
        public UserNotificationPreferencesController(IUserNotificationPreferencesService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtiene las preferencias de notificación del usuario autenticado.
        /// </summary>
        //  /// </summary>
        [HttpGet("me")]
        public async Task<ActionResult<UserNotificationPreferencesDto>> GetMyPreferences()
        {
            int userId = int.Parse(User.FindFirst("id")!.Value);
            var prefs = await _service.GetByUserIdAsync(userId);
            var dto = new UserNotificationPreferencesDto
            {
                ReceiveEmailNotifications = prefs.ReceiveEmailNotifications,
                ReceiveInternalNotifications = prefs.ReceiveInternalNotifications,
                NotifyOnComment = prefs.NotifyOnComment,
                NotifyOnAdminMessage = prefs.NotifyOnAdminMessage,
                NotifyOnSystemAlert = prefs.NotifyOnSystemAlert,
            };
            return Ok(dto);
        }

        /// <summary>
        /// Actualiza las preferencias de notificación del usuario autenticado.
        /// </summary>
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyPreferences(
            [FromBody] UserNotificationPreferencesDto dto
        )
        {
            int userId = int.Parse(User.FindFirst("id")!.Value);
            await _service.UpdateAsync(userId, dto);
            return NoContent();
        }
    }
}
