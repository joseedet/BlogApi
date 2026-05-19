using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.DTO;

/// <summary>
/// Clase que representa un DTO (Data Transfer Object) para un mensaje de contacto. Este DTO se utiliza para transferir los datos de un mensaje de contacto entre la capa de presentación y la capa de negocio de la aplicación. Contiene propiedades para el identificador, nombre, correo electrónico, asunto, mensaje y fecha de creación del mensaje de contacto, que son los campos necesarios para representar un mensaje de contacto en la aplicación.
/// </summary>
public class MensajeContactoDto
{
    /// <summary>
    /// Identificador del mensaje de contacto. Este campo es obligatorio y se utiliza para identificar únicamente el mensaje de contacto en la base de datos.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del remitente del mensaje de contacto. Este campo es obligatorio y se utiliza para identificar quién envió el mensaje.
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Dirección de correo electrónico del remitente del mensaje de contacto. Este campo es obligatorio y se utiliza para contactar con el remitente.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Asunto del mensaje de contacto. Este campo es obligatorio y se utiliza para indicar el tema del mensaje.
    /// </summary>
    public string Asunto { get; set; } = string.Empty;

    /// <summary>
    /// Contenido del mensaje de contacto. Este campo es obligatorio y se utiliza para proporcionar los detalles del mensaje.
    /// </summary>
    public string Mensaje { get; set; } = string.Empty;

    /// <summary>
    /// Fecha y hora de creación del mensaje de contacto. Este campo es obligatorio y se utiliza para registrar cuándo se creó el mensaje.
    /// </summary>
    public DateTime FechaCreacion { get; set; }
}
