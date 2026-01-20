using BlogApi.DTO;
using BlogApi.Models;

/// <summary>
/// Interfaz para EmailSettingsService
/// </summary>
public interface IEmailSettingsService
{
    /// <summary>
    /// Obtenemos los emails registrados
    /// </summary>
    /// <returns>EmailSettingsDto</returns>
    Task<EmailSettingsDto> ObtenerAsync();

    /// <summary>
    /// Actualiza el email seleccionado
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>EmailSettingsDto</returns>
    Task<EmailSettingsDto> ActualizarAsync(EmailSettingsUpdateDto dto);

    /// <summary>
    /// Obtenemos la
    /// </summary>
    /// <returns></returns>
    Task<EmailSettings> ObtenerEntidadAsync();
}
