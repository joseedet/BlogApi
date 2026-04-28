using System;
using System.ComponentModel.DataAnnotations;

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

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico del remitente del mensaje de contacto. Este campo es obligatorio y se utiliza para contactar con el remitente.
    /// </summary>

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    [StringLength(150, ErrorMessage = "El email no puede superar los 150 caracteres.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Asunto del mensaje de contacto. Este campo es obligatorio y se utiliza para resumir el contenido del mensaje.
    /// </summary>

    [Required(ErrorMessage = "El asunto es obligatorio.")]
    [StringLength(100, ErrorMessage = "El asunto no puede superar los 100 caracteres.")]
    public string Asunto { get; set; } = string.Empty;

    /// <summary>
    /// Contenido del mensaje de contacto. Este campo es obligatorio y se utiliza para almacenar el texto del mensaje.
    /// </summary>
    [Required(ErrorMessage = "El mensaje es obligatorio.")]
    [EmailAddress(ErrorMessage = "El mensaje no tiene un formato válido.")]
    [StringLength(2000, ErrorMessage = "El mensaje no puede superar los 2000 caracteres.")]
    
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
