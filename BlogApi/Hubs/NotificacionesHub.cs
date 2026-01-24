using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BlogApi.Hubs;

public class NotificacionesHub : Hub
{
    private readonly ILogger<NotificacionesHub> _logger;

    public NotificacionesHub(ILogger<NotificacionesHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;

        _logger.LogInformation("Usuario {UserId} conectado al hub de notificaciones", userId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;

        _logger.LogInformation("Usuario {UserId} desconectado del hub de notificaciones", userId);

        if (exception != null)
        {
            _logger.LogWarning(exception, "Desconexión inesperada del usuario {UserId}", userId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
