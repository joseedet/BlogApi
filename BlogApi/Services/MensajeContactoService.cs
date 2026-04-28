using BlogApi.Models;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services.Interfaces;



namespace BlogApi.Services;

/// <summary>
/// Servicio para manejar los mensajes de contacto, implementa la interfaz IMensajeContactoService y define los métodos para crear un mensaje de contacto, obtener un mensaje por su ID y obtener todos los mensajes de contacto. Esta clase utiliza el repositorio de mensajes de contacto para interactuar con la base de datos y manejar la lógica de negocio relacionada con los mensajes de contacto.
/// </summary>
public class MensajeContactoService : IMensajeContactoService
{
    private readonly IMensajeContactoRepository _repositorio;

    /// <summary>
    /// Constructor para el servicio de mensajes de contacto, recibe una instancia de IMensajeContactoRepository para interactuar con la base de datos a través del repositorio. Esta instancia es inyectada a través del constructor y se utiliza en los métodos del servicio para realizar las operaciones relacionadas con los mensajes de contacto.
    /// </summary>
    /// <param name="repositorio"></param>
    public MensajeContactoService(IMensajeContactoRepository repositorio)
    {
        _repositorio = repositorio;
    }

    /// <summary>
    /// Crea un nuevo mensaje de contacto utilizando el repositorio para guardar el mensaje en la base de datos. Este método recibe un objeto MensajeContacto que contiene la información del mensaje a crear y devuelve el mensaje creado con su ID asignado por la base de datos.
    /// </summary>
    /// <param name="mensaje"></param>
    /// <returns></returns>

    public async Task<MensajeContacto> CrearMensajeAsync(MensajeContacto mensaje)
    {
        if (string.IsNullOrWhiteSpace(mensaje.Nombre))
            throw new ArgumentException("El nombre es obligatorio.");

        if (string.IsNullOrWhiteSpace(mensaje.Email))
            throw new ArgumentException("El email es obligatorio.");

        if (string.IsNullOrWhiteSpace(mensaje.Mensaje))
            throw new ArgumentException("El mensaje es obligatorio.");
        if (string.IsNullOrWhiteSpace(mensaje.Asunto))
            throw new ArgumentException("El asunto es obligatorio.");
        if (mensaje.Mensaje.Length > 2000)
            throw new ArgumentException("El mensaje no puede superar los 2000 caracteres.");

        return await _repositorio.CrearAsync(mensaje);
    }
    
    /// <summary>
    /// Obtiene un mensaje de contacto por su ID utilizando el repositorio para buscar el mensaje en la base de datos. Este método recibe el ID del mensaje a buscar y devuelve el mensaje encontrado o null si no se encuentra ningún mensaje con ese ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>

    public async Task<MensajeContacto?> ObtenerMensajePorIdAsync(int id)
    {
        return await _repositorio.ObtenerPorIdAsync(id);
    }

    /// <summary>
    /// Obtiene todos los mensajes de contacto utilizando el repositorio para recuperarlos de la base de datos. Este método no recibe parámetros y devuelve una lista con todos los mensajes de contacto disponibles.
    /// </summary>
    /// <returns></returns>
    public async Task<List<MensajeContacto>> ObtenerMensajesAsync()
    {
        return await _repositorio.ObtenerTodosAsync();
    }
}
