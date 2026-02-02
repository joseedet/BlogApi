using BlogApi.Models;

namespace BlogApi.Services.Interfaces;
/// <summary>
/// Interfaz de servicio de categoria
/// </summary>
public interface ICategoriaService
{
    /// <summary>
    /// Obtener todos
    /// </summary>
    /// <returns>IEnumerable&lt;Categoria&gt;</returns>
    Task<IEnumerable<Categoria>> GetAllAsync();

    /// <summary>
    /// Obtener por id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Categoria?</returns>
    Task<Categoria?> GetByIdAsync(int id);

    /// <summary>
    /// Crea una categoria
    /// </summary>
    /// <param name="categoria"></param>
    /// <returns>Categoria</returns>
    Task<Categoria> CreateAsync(Categoria categoria);

    /// <summary>
    /// Actualiza categoría
    /// </summary>
    /// <param name="id"></param>
    /// <param name="categoria"></param>
    /// <returns>Devuelve verdadero si se ha actualizado de lo contrario falso</returns>
    Task<bool> UpdateAsync(int id, Categoria categoria);

    /// <summary>
    /// Elimina categoría
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Devuelve verdadero si se ha actualizado de lo contrario falso</returns>
    Task<bool> DeleteAsync(int id);

    
}
