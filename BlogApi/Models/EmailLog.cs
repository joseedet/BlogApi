/// <summary>
/// Clase de log para el email
/// </summary>
public class EmailLog
{
    /// <summary>
    /// Identificador
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Destinatario del email
    /// </summary>
    public string Destinatario { get; set; } = string.Empty;

    /// <summary>
    /// Asunto del email
    /// </summary>
    public string Asunto { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de envio el correo
    /// </summary>
    public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Si el envio ha sido exitoso
    /// </summary>
    public bool Exito { get; set; }

    /// <summary>
    /// Error ocurrido al enviar el correo
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Proveedor utilizado para enviar el correo
    /// </summary>
    public string? Proveedor { get; set; } // "SendGrid", "SMTP", etc.
}
