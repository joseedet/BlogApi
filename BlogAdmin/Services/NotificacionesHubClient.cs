using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client; // ← FALTA ESTO

namespace BlogAdmin.Services;

public class NotificacionesHubClient
{
    private HubConnection? _connection;
    public event Action<string, DateTime>? OnNotificacion;

    public async Task IniciarAsync(string token)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(
                "https://tublog.com/hubs/notificaciones",
                options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                }
            )
            .WithAutomaticReconnect()
            .Build();

        _connection.On<object>(
            "NuevaNotificacion",
            data =>
            {
                var mensaje = data.GetType().GetProperty("mensaje")?.GetValue(data)?.ToString();
                var fecha = (DateTime)(
                    data.GetType().GetProperty("fecha")?.GetValue(data) ?? DateTime.UtcNow
                );

                OnNotificacion?.Invoke(mensaje ?? "", fecha);
            }
        );

        await _connection.StartAsync();
    }
}
