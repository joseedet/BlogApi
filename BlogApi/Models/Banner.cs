using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Clase Banner
/// </summary>
public class Banner
{
    /// <summary>
    /// Identificación única del banner
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Título del banner
    /// </summary>
    [Required, MaxLength(150)]
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Subtitulo del banner
    /// </summary>
    [MaxLength(250)]
    public string Subtitulo { get; set; } = string.Empty;

    /// <summary>
    /// Url de la imagen
    /// </summary>
    [Required]
    public string ImagenUrl { get; set; } = string.Empty;

    /// <summary>
    /// Enlace del banner
    /// </summary>
    [MaxLength(300)]
    public string Enlace { get; set; } = string.Empty;

    // Estado manual

    /// <summary>
    /// Estado en el que se encuentra el banner
    /// </summary>
    public bool Activo { get; set; } = true;

    // Programación automática

    /// <summary>
    /// Fecha de inicio de la ejecución  del banner
    /// </summary>
    public DateTime? FechaInicio { get; set; }

    /// <summary>
    /// Fecha final de la ejecución del banner
    /// </summary>
    public DateTime? FechaFin { get; set; }

    // Orden de aparición (útil para sliders)

    /// <summary>
    /// Orden en el cual se ejecutan los banners
    /// </summary>
    public int Orden { get; set; } = 0;

    // Para subir imagen desde el panel admin

    /// <summary>
    /// Para subir la imagen
    /// </summary>
    [NotMapped]
    public IFormFile? ImagenFile { get; set; }

    /// <summary>
    /// Texto alternativo(SEO/accesibilidad)
    /// </summary>
    [MaxLength(200)]
    public string Alt { get; set; } = string.Empty;

    /// <summary>
    /// Target del enlace
    /// </summary>
    public bool AbrirEnNuevaPestana { get; set; } = false;

    /// <summary>
    /// Descripción
    /// </summary>
    [MaxLength(500)]
    public string Descripcion { get; set; } = string.Empty;
}
