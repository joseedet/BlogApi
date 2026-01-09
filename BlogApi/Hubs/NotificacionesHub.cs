using Microsoft.AspNetCore.SignalR;

namespace BlogApi.Hubs;

public class NotificacionesHub : Hub
{
    /// <summary>
    /// Método llamado cuando un cliente se conecta al hub
    /// </summary>
    /// <returns></returns>
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }
}
