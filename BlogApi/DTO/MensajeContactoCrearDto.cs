using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.DTO;
/// <summary>
/// Clase que representa un DTO (Data Transfer Object) para la creación de un mensaje de contacto. Este DTO se utiliza para recibir los datos necesarios para crear un nuevo mensaje de contacto a través de la API. Contiene propiedades para el nombre, correo electrónico, asunto y mensaje del contacto, que son los campos requeridos para crear un nuevo mensaje de contacto en la base de datos.
/// </summary>
public class MensajeContactoCrearDto
{
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
}
