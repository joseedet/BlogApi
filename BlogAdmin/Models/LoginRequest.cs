namespace BlogAdmin.Models;
/// <summary>
/// Clase que representa la solicitud de inicio de sesión en la aplicación de administración del blog. Contiene las propiedades necesarias para que un usuario pueda autenticarse, como el correo electrónico y la contraseña. Esta clase se utiliza para enviar los datos de inicio de sesión al servidor y obtener un token de autenticación en respuesta.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Correo electrónico del usuario que intenta iniciar sesión. Este campo es obligatorio y se utiliza para identificar al usuario en el sistema de autenticación. El correo electrónico debe ser válido y estar registrado en la base de datos para que el inicio de sesión sea exitoso.    
    /// </summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// Contraseña del usuario que intenta iniciar sesión. Este campo es obligatorio y se utiliza para verificar la identidad del usuario en el sistema de autenticación. La contraseña debe ser segura y cumplir con los requisitos establecidos por la aplicación.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
