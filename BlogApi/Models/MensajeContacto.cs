using System;

namespace BlogApi.Models;

/// <summary>
/// Clase que representa un mensaje de contacto enviado por un usuario a través del formulario de contacto en la aplicación.
/// </summary>
public class MensajeContacto
{
    /// <summary>
    /// Identificador único del mensaje de contacto. Este campo es la clave primaria en la base de datos.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del remitente del mensaje de contacto. Este campo es obligatorio y se utiliza para identificar quién envió el mensaje.
    /// </summary>    

    public string Nombre { get; set; } = string.Empty;
        
    /// <summary>
    /// Correo electrónico del remitente del mensaje de contacto. Este campo es obligatorio y se utiliza para contactar con el remitente.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Asunto del mensaje de contacto. Este campo es obligatorio y se utiliza para resumir el contenido del mensaje.
    /// </summary>
    public string Asunto { get; set; } = string.Empty;

    /// <summary>
    /// Contenido del mensaje de contacto. Este campo es obligatorio y se utiliza para almacenar el texto del mensaje.
    /// </summary>
    public string Mensaje { get; set; } = string.Empty;

    /// <summary>
    /// Fecha y hora de creación del mensaje de contacto. Este campo es automático y se utiliza para registrar cuándo se envió el mensaje.
    /// </summary>
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Dirección IP del remitente del mensaje de contacto. Este campo es opcional y se utiliza para registrar la dirección IP desde la que se envió el mensaje.
    /// </summary>
    public string? DireccionIp { get; set; }
}
