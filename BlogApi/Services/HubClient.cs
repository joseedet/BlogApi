using System.Data;
using System.Net.Http.Headers;
using System.Text.Json;
using BlogApi.DTO;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlogApi.Services;
/// <summary>
/// Servicio para manejar la conexión al Hub de notificaciones utilizando SignalR. Esta clase define métodos para iniciar y detener la conexión al Hub, así como un evento que se dispara cuando se recibe una notificación del backend. El método IniciarAsync establece la conexión al Hub utilizando un token JWT para autenticación, mientras que el método DetenerAsync cierra la conexión. El evento OnNotificacion permite a otras partes de la aplicación, como MainLayout, escuchar y reaccionar a las notificaciones recibidas del backend.
/// </summary>
public class HubClient
{
    /// <summary>
    /// Instancia de HubConnection que representa la conexión al Hub de notificaciones. Esta variable se utiliza para gestionar la conexión, enviar y recibir mensajes del Hub. Se inicializa en el método IniciarAsync y se detiene en el método DetenerAsync. La conexión se establece utilizando un token JWT para autenticación y se configura para reconectarse automáticamente en caso de desconexión.
    /// </summary>
    private HubConnection? _hub;

    // Evento que MainLayout escuchará para mostrar las notificaciones
   /// <summary>
   /// Evento que se dispara cuando se recibe una notificación del backend a través del Hub de SignalR. Este evento permite a otras partes de la aplicación, como MainLayout, escuchar y reaccionar a las notificaciones recibidas. El evento utiliza un delegado Action que recibe un objeto NotificacionDto, el cual contiene la información de la notificación recibida. Cuando se recibe una notificación, el método registrado en este evento se ejecutará con los datos de la notificación para mostrarla al usuario o realizar otras acciones según sea necesario.
   /// </summary>

    public event Action<NotificacionDto>? OnNotificacion;

    /// <summary>
    /// Inicia la conexión al Hub de notificaciones usando el token JWT.
    /// </summary>
    public async Task IniciarAsync(string token)
    {
        if (_hub != null && _hub.State == HubConnectionState.Connected)
            return;

        _hub = new HubConnectionBuilder()
            .WithUrl(
                "https://TU_API/hubs/notificaciones",
                options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult((string?)token);
                }
            )
            .WithAutomaticReconnect()
            .Build();

        // Escuchar notificaciones del backend
        _hub.On<object>(
            "NotificacionRecibida",
            data =>
            {
                try
                {
                    // Convertimos el objeto recibido a NotificacionDto
                    var json = JsonSerializer.Serialize(data);
                    var notificacion = JsonSerializer.Deserialize<NotificacionDto>(json);

                    if (notificacion != null)
                        OnNotificacion?.Invoke(notificacion);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al procesar notificación: " + ex.Message);
                }
            }
        );

        await _hub.StartAsync();
    }

    /// <summary>
    /// Cierra la conexión al Hub.
    /// </summary>
    public async Task DetenerAsync()
    {
        if (_hub != null)
            await _hub.StopAsync();
    }
}
