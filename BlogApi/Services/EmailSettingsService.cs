using BlogApi.Data;
using BlogApi.DTO;
using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Clase de servicio de EmailSettingsService
/// </summary>
public class EmailSettingsService : IEmailSettingsService
{
    private readonly BlogDbContext _db;

    /// <summary>
    /// Constructor de EmailSettingsService
    /// </summary>
    /// <param name="db"></param>
    public EmailSettingsService(BlogDbContext db)
    {
        _db = db;
    }

    // ------------------------------------------------------------
    // Obtener configuración (si no existe, crearla)
    // ------------------------------------------------------------

    /// <summary>
    /// Obtener emails
    /// </summary>
    /// <returns>EmailSettingsDto</returns>
    public async Task<EmailSettingsDto> ObtenerAsync()
    {
        var settings = await _db.EmailSettings.FirstOrDefaultAsync();

        if (settings == null)
        {
            settings = new EmailSettings
            {
                Host = "",
                Puerto = 587,
                Usuario = "",
                Password = "",
                Remitente = "",
                NombreRemitente = "",
                UsarSSL = true,
                Activo = false,
            };

            _db.EmailSettings.Add(settings);
            await _db.SaveChangesAsync();
        }

        return ToDto(settings);
    }

    // ------------------------------------------------------------
    // Actualizar configuración
    // ------------------------------------------------------------

    /// <summary>
    /// Actualizar email
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>EmailSettingsDto</returns>
    public async Task<EmailSettingsDto> ActualizarAsync(EmailSettingsUpdateDto dto)
    {
        var settings = await _db.EmailSettings.FirstOrDefaultAsync();

        if (settings == null)
        {
            settings = new EmailSettings();
            _db.EmailSettings.Add(settings);
        }

        settings.Host = dto.Host;
        settings.Puerto = dto.Puerto;
        settings.Usuario = dto.Usuario;
        settings.Password = dto.Password;
        settings.Remitente = dto.Remitente;
        settings.NombreRemitente = dto.NombreRemitente;
        settings.UsarSSL = dto.UsarSSL;
        settings.Activo = dto.Activo;

        await _db.SaveChangesAsync();

        return ToDto(settings);
    }

    // ------------------------------------------------------------
    // Conversión a DTO
    // ------------------------------------------------------------
    private static EmailSettingsDto ToDto(EmailSettings s) =>
        new()
        {
            Host = s.Host,
            Puerto = s.Puerto,
            Usuario = s.Usuario,
            Remitente = s.Remitente,
            NombreRemitente = s.NombreRemitente,
            UsarSSL = s.UsarSSL,
            Activo = s.Activo,
        };
}
