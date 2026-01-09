using System.Data;
using System.Net.Http.Headers;
using System.Text.Json;
using BlogApi.DTO;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlogApi.Services;

public class HubClient
{
    private HubConnection? _hub;

    // Evento que MainLayout escuchará
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
