using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Models;

namespace BlogApi.DTO;

/// <summary>
/// DTO para paginación genérica
/// </summary>
public class PaginationDto<T>
{
    /// <summary>
    /// Elementos de la página actual
    /// </summary>
    public IEnumerable<T> Items { get; set; }

    /// <summary>
    /// Total de elementos disponibles
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Página actual
    /// </summary>
    public int Pagina { get; set; }

    /// <summary>
    /// Tamaño de página
    /// </summary>
    public int Tamano { get; set; }

    /// <summary>
    /// Constructor de PaginationDto
    /// </summary>
    /// <param name="items"></param>
    /// <param name="total"></param>
    /// <param name="pagina"></param>
    /// <param name="tamano"></param>
    /// <returns>IEnumerable&lt;T&gt;</returns>
    public PaginationDto(IEnumerable<T> items, int total, int pagina, int tamano)
    {
        Items = items;
        Total = total;
        Pagina = pagina;
        Tamano = tamano;
    }

    /// <summary>
    /// /// Constructor vacío de PaginationDto
    /// </summary>
    public PaginationDto() { }
}
