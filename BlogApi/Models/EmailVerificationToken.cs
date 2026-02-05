using System;

namespace BlogApi.Models;

/// <summary>
/// Email verification Token
/// </summary>
public class EmailVerificationToken
{
    /// <summary>
    /// Identificador
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del usuario
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Propiedad de navegación de usuario
    /// </summary>
    public Usuario Usuario { get; set; }

    /// <summary>
    /// Token
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Fecha de expiración token
    /// </summary>
    public DateTime ExpiraEn { get; set; }

    /// <summary>
    /// Si ha sido usado
    /// </summary>
    public bool Usado { get; set; }

    /// <summary>
    /// Usado en la fecha
    /// </summary>
    public DateTime? UsadoEn { get; set; }

    /// <summary>
    /// Ip Creacion
    /// </summary>
    public string IpCreacion { get; set; }

    /// <summary>
    /// User-Agent de creación
    /// </summary>
    public string UserAgentCreacion { get; set; }

    /// <summary>
    /// Ip usada
    /// </summary>
    public string IpUso { get; set; }

    /// <summary>
    /// User-Agent usado
    /// </summary>
    public string UserAgentUso { get; set; }

    /// <summary>
    /// Fecha de creación
    /// </summary>
    public DateTime CreadoEn { get; set; }

    /// <summary>
    /// Número de reenvios de Token
    /// </summary>
    public int Reenvios { get; set; }

    /// <summary>
    /// Hash Token
    /// </summary>
    // Hash del token: SHA512(token + salt)
    public string TokenHash { get; set; }
}
