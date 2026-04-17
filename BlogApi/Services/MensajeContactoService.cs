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

    public async Task<MensajeContacto> CrearMensajeAsync(MensajeContacto mensaje)
    {
        return await _repositorio.CrearAsync(mensaje);
    }
    

    public async Task<MensajeContacto?> ObtenerMensajePorIdAsync(int id)
    {
        return await _repositorio.ObtenerPorIdAsync(id);
    }

    public async Task<List<MensajeContacto>> ObtenerMensajesAsync()
    {
        return await _repositorio.ObtenerTodosAsync();
    }
}
