using Microsoft.AspNetCore.SignalR;

namespace BlogApi.Hubs;

public class NotificacionesHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }
}
