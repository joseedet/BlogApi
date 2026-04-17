using BlogApi.Models;

namespace BlogApi.Repositories.Interfaces;


/// <summary>
/// Interfaz para el repositorio de mensajes de contacto, define los métodos para crear, obtener por ID, obtener todos y guardar cambios en los mensajes de contacto.   
/// </summary>
public interface IMensajeContactoRepository
{
    /// <summary>
    /// Crea un nuevo mensaje de contacto.
    /// </summary>
    public interface IMensajeContactoRepository
    {
        /// <summary>
        /// Crea un nuevo mensaje de contacto.  
        /// </summary>
        /// <param name="mensaje"></param>
        /// <returns>MensajeContacto</returns>
        Task<MensajeContacto> CrearAsync(MensajeContacto mensaje);
        
        /// <summary>
        /// Obtiene un mensaje de contacto por su ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>MensajeContacto</returns>
        Task<MensajeContacto?> ObtenerPorIdAsync(int id);


        /// <summary>
        /// Obtiene todos los mensajes de contacto.
        /// </summary>
        /// <returns>Lista de MensajeContacto</returns>
        Task<List<MensajeContacto>> ObtenerTodosAsync();

        /// <summary>
        /// Guarda los cambios realizados en los mensajes de contacto.
        /// </summary>       
        Task GuardarCambiosAsync();
    }
}
