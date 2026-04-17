using BlogApi.Data;
using BlogApi.Models;
using Microsoft.EntityFrameworkCore;
using BlogApi.Repositories.Interfaces;

namespace BlogApi.Repositories;

/// <summary>
/// Repositorio para manejar los mensajes de contacto, implementa la interfaz IMensajeContactoRepository y define los métodos para crear, obtener por ID, obtener todos y guardar cambios en los mensajes de contacto.
/// </summary>
public class MensajeContactoRepository : IMensajeContactoRepository
{
    private readonly BlogDbContext _context;
        /// <summary>
        /// Constructor para el repositorio de mensajes de contacto, recibe una instancia de BlogDbContext para interactuar con la base de datos.
        /// </summary>
        /// <param name="context"></param>
        public MensajeContactoRepository(BlogDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Crea un nuevo mensaje de contacto, agrega el mensaje a la base de datos y guarda los cambios. Devuelve el mensaje creado con su ID asignado por la base de datos.
        /// </summary>
        /// <param name="mensaje"></param>
        /// <returns>MensajeContacto</returns>
        public async Task<MensajeContacto> CrearAsync(MensajeContacto mensaje)
        {
            _context.MensajesContacto.Add(mensaje);
            await _context.SaveChangesAsync();
            return mensaje;
        }
        /// <summary>
        /// Obtiene un mensaje de contacto por su ID, busca el mensaje en la base de datos y devuelve el mensaje encontrado o null si no se encuentra ningún mensaje con el ID proporcionado.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>MensajeContacto</returns>
        public async Task<MensajeContacto?> ObtenerPorIdAsync(int id)
        {
            return await _context.MensajesContacto
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        /// <summary>
        /// Obtiene todos los mensajes de contacto, consulta la base de datos para obtener una lista de todos los mensajes de contacto ordenados por fecha de creación en orden descendente y devuelve la lista resultante.
        /// </summary>
        /// <returns>Lista de MensajeContacto</returns>
        public async Task<List<MensajeContacto>> ObtenerTodosAsync()
        {
            return await _context.MensajesContacto
                .OrderByDescending(x => x.FechaCreacion)
                .ToListAsync();
        }
        /// <summary>
        /// Guarda los cambios realizados en los mensajes de contacto, llama al método SaveChangesAsync del contexto para persistir los cambios en la base de datos.
        /// </summary>
        
        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
}
