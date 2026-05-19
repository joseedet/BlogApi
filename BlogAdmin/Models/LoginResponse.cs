namespace BlogAdmin.Models;

/// <summary>
/// Clase que representa la respuesta de inicio de sesión en la aplicación de administración del blog. Contiene la propiedad Token, que es una cadena que representa el token de autenticación generado por el servidor después de un inicio de sesión exitoso. Este token se utiliza para autenticar las solicitudes posteriores del usuario en la aplicación y mantener su sesión activa. La propiedad Token es esencial para garantizar la seguridad y el acceso controlado a los recursos protegidos de la aplicación.
/// </summary>
public class LoginResponse
{
/// <summary>
/// Token de autenticación generado por el servidor después de un inicio de sesión exitoso. Este token es una cadena que se utiliza para autenticar las solicitudes posteriores del usuario en la aplicación y mantener su sesión activa. La propiedad Token es esencial para garantizar la seguridad y el acceso controlado a los recursos protegidos de la aplicación.
/// </summary>
    public string Token { get; set; } = string.Empty;
}
