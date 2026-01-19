using BlogApi.Utils;

namespace BlogApi.DTO;

/// <summary>
/// Clase BannerDto
/// </summary>
public class BannerDto
{
    /// <summary>
    /// Identificación única del banner
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Título del banner
    /// </summary>
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Subtitulo del banner
    /// </summary>
    public string Subtitulo { get; set; } = string.Empty;

    /// <summary>
    /// Url de la imagen
    /// </summary>
    public string ImagenUrl { get; set; } = string.Empty;

    /// <summary>
    /// Enlace del banner
    /// </summary>
    public string Enlace { get; set; } = string.Empty;

    /// <summary>
    /// Estado en el que se encuentra el banner
    /// </summary>
    public bool Activo { get; set; }

    /// <summary>
    /// Fecha de inicio de la ejecución  del banner
    /// </summary>
    public DateTime? FechaInicio { get; set; }

    /// <summary>
    /// Fecha final de la ejecución del banner
    /// </summary>
    public DateTime? FechaFin { get; set; }

    /// <summary>
    /// Orden en el cual se ejecutan los banners
    /// </summary>
    public int Orden { get; set; }

    /// <summary>
    /// Texto alternativo(SEO/accesibilidad)
    /// </summary>
    public string Alt { get; set; } = string.Empty;

    /// <summary>
    /// Target del enlace
    /// </summary>
    public bool AbrirEnNuevaPestana { get; set; }

    /// <summary>
    /// Descripción
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de Banner
    /// </summary>
    public TipoBanner Tipo { get; set; }
}
