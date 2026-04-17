using BlogApi.Models;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Interface para menú servicio dónde se define el contrato para obtener los elementos del menú de la aplicación.
/// </summary>
public interface IMenuService
{
     /// <summary>
     /// Método asíncrono para obtener la lista de elementos del menú.
     /// </summary>
     /// <returns></returns>
     Task<List<MenuItem>> GetMenuAsync();
}
