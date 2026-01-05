using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlogAdmin.Services;

public class NotificacionesHubClient
{
    private HubConnection? _connection;
    public event Action<NotificacionDto>? OnNotificacion;

    public async Task IniciarAsync(string token)
    {
        if (_connection is not null && _connection.State == HubConnectionState.Connected)
            return;

        _connection = new HubConnectionBuilder()
            .WithUrl(
                "https://tublog.com/hubs/notificaciones",
                options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token)!;
                }
            )
            .WithAutomaticReconnect()
            .Build();

        _connection.On<object>(
            "NuevaNotificacion",
            data =>
            {
                var tipo = data.GetType();
                var mensaje =
                    tipo.GetProperty("mensaje")?.GetValue(data)?.ToString() ?? string.Empty;
                var fechaObj = tipo.GetProperty("fecha")?.GetValue(data);
                var fecha = fechaObj is DateTime dt ? dt : DateTime.UtcNow;
                var id = tipo.GetProperty("id")?.GetValue(data)?.ToString();

                var notificacion = new NotificacionDto
                {
                    Mensaje = mensaje,
                    Fecha = fecha,
                    Leida = false,
                    Id = id,
                };

                OnNotificacion?.Invoke(notificacion);
            }
        );

        await _connection.StartAsync();
    }
}
