using System;

namespace BlogApi.DTO;

/// <summary>
/// DTO para editar un permiso, contiene la clave única del permiso y su descripción, utilizado para actualizar los permisos de un rol específico en el controlador de roles
/// </summary>
public class EditarPermisoDto
{
    /// <summary>
    /// Clave única del permiso, por ejemplo: "Usuarios.Editar", "Banners.Crear", utilizada para identificar el permiso que se asignará o eliminará de un rol, debe ser única y descriptiva del permiso que representa
    /// </summary>
    public string Clave { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del permiso, por ejemplo: "Permite editar usuarios", "Permite crear banners", utilizada para proporcionar una descripción legible del permiso, no es obligatoria pero puede ayudar a entender el propósito del permiso al asignarlo a un rol o al mostrarlo en la interfaz de administración de roles y permisos
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;
}
