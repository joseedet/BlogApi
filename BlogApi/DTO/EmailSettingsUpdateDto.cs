/// <summary>
/// Clase EmailSettingUpdateDto
/// </summary>
public class EmailSettingsUpdateDto
{
    /// <summary>
    /// Host
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// Puerto del host
    /// </summary>
    public int Puerto { get; set; }

    /// <summary>
    /// Usuario de email
    /// </summary>
    public string Usuario { get; set; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Remitente
    /// </summary>
    public string Remitente { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del remitente
    /// </summary>
    public string NombreRemitente { get; set; } = string.Empty;

    /// <summary>
    /// Usar email seguro
    /// </summary>
    public bool UsarSSL { get; set; }

    /// <summary>
    /// Está activa la cuenta de email.
    /// </summary>
    public bool Activo { get; set; }
}
