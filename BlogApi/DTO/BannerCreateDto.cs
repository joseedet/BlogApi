using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Utils;

namespace BlogApi.DTO;

/// <summary>
/// Clase Dto de creación de Banner
/// </summary>
public class BannerCreateDto
{
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
    public IFormFile ImagenFile { get; set; } = default!;

    /// <summary>
    /// Enlace del banner
    /// </summary>
    [MaxLength(300)]
    public string Enlace { get; set; } = string.Empty;

    /// <summary>
    /// Estado en el que se encuentra el banner
    /// </summary>
    public bool Activo { get; set; } = true;

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
    public int Orden { get; set; } = 0;

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

    /// <summary>
    /// Tipo de Banner
    /// </summary>
    public TipoBanner Tipo { get; set; } = TipoBanner.Slider;
}
