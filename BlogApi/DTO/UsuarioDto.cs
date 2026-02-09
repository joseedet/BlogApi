using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Models;

namespace BlogApi.DTO;

/// <summary>
/// UsuarioDto
/// </summary>
public class UsuarioDto
{
    /// <summary>
    /// Identificador usuario dti
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Apellidos
    /// </summary>
    public string Apellidos { get; set; } = string.Empty;

    /// <summary>
    /// Está bloqueado
    /// </summary>
    public bool EstaBloqueado { get; set; }

    /// <summary>
    /// Indica si el email del usuario ha sido verificado. Es un campo importante para determinar si el usuario ha completado el proceso de registro y verificación de su cuenta. Si es true, significa que el usuario ha verificado su email; si es false, significa que el usuario aún no ha verificado su email.
    /// </summary>
    public bool EmailVerificado { get; set; }

    /// <summary>
    /// Url del avatar del usuario. Es un campo opcional que puede contener la URL de una imagen que representa al usuario. Si se proporciona, se puede utilizar para mostrar el avatar del usuario en la interfaz de usuario. Si no se proporciona, se puede mostrar un avatar predeterminado o no mostrar ningún avatar.
    /// </summary>
    public string AvatarUrl { get; set; } = string.Empty;

    /// <summary>
    /// Roles del usuario. Es una lista de cadenas que representa los roles asignados al usuario. Cada cadena en la lista representa el nombre de un rol, como "Admin", "Autor" o "Lector". Esta información es útil para determinar los permisos y el acceso que tiene el usuario dentro del sistema. Por ejemplo, un usuario con el rol "Admin" podría tener acceso a todas las funcionalidades del sistema, mientras que un usuario con el rol "Lector" podría tener acceso limitado a ciertas funcionalidades.
    /// </summary>
    public List<string> Roles { get; set; } = new();
}
