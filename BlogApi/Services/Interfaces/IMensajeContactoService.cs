using System;
using BlogApi.Models;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Interfaz para el servicio de mensajes de contacto, define los métodos para crear un mensaje de
/// contacto, obtener un mensaje por su ID y obtener todos los mensajes de contacto. Esta interfaz es implementada por la clase MensajeContactoService para manejar la lógica de negocio relacionada con los mensajes de contacto.
/// </summary>
public interface IMensajeContactoService
{
    /// <summary>
    /// Crea un nuevo mensaje de contacto, recibe un objeto MensajeContacto con los datos del mensaje a crear, llama al método CrearAsync del repositorio para guardar el mensaje en la base de datos y devuelve el mensaje creado con su ID asignado por la base de datos.
    /// </summary>
    /// <param name="mensaje"></param>
    /// <returns>MensajeContacto</returns>
    Task<MensajeContacto> CrearMensajeAsync(MensajeContacto mensaje);

    /// <summary>
    /// Obtiene un mensaje de contacto por su ID, recibe el ID del mensaje a obtener
    /// y llama al método ObtenerPorIdAsync del repositorio para buscar el mensaje en la base de datos. Devuelve el mensaje encontrado o null si no se encuentra ningún mensaje con el ID proporcionado.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>MensajeContacto</returns>
    Task<MensajeContacto?> ObtenerMensajePorIdAsync(int id);

    /// <summary>
    /// Obtiene todos los mensajes de contacto, llama al método ObtenerTodosAsync del repositorio para consultar la base de datos y obtener una lista de todos los mensajes de contacto. Devuelve la lista resultante de mensajes de contacto.
    /// </summary>
    /// <returns>Lista de MensajeContacto</returns>
    Task<List<MensajeContacto>> ObtenerMensajesAsync();
}
