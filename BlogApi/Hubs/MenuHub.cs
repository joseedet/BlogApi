using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR; 
namespace BlogApi.Hubs;

/// <summary>
/// Hub de SignalR para notificar a los clientes cuando el menú ha sido actualizado. Esto permite que las aplicaciones cliente puedan actualizar su menú en tiempo real sin necesidad de recargar la página. Cuando se llama al método NotifyMenuUpdated, se envía un mensaje a todos los clientes conectados indicando que el menú ha sido actualizado, lo que les permite reaccionar y actualizar su interfaz de usuario en consecuencia.
/// </summary>
public class MenuHub : Hub
{
    public async Task NotifyMenuUpdated()
    {
        await Clients.All.SendAsync("MenuUpdated");
    }
}
