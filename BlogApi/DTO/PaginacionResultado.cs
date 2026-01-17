using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.DTO;

/// <summary>
/// Resultado de paginación
/// </summary>
/// <typeparam name="T"></typeparam>
public class PaginacionResultado<T>
{
    /// <summary>
    /// Elementos de la página actual
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Información de paginación
    /// </summary>
    public int PaginaActual { get; set; }

    /// <summary>
    /// Número total de páginas
    /// </summary>
    public int TotalPaginas { get; set; }

    /// <summary>
    /// Número total de registros
    /// </summary>
    public int TotalRegistros { get; set; }
}
