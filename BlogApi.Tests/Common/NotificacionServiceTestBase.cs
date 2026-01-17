using BlogApi.Data;
using BlogApi.Hubs;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BlogApi.Tests.Common;

public abstract class NotificacionServiceTestBase
{
    protected BlogDbContext Db = null!;
    protected readonly Mock<INotificacionRepository> Repo = new();
    protected readonly Mock<IHubContext<NotificacionesHub>> Hub = new();
    protected readonly Mock<IClientProxy> ClientProxy = new();
    protected readonly Mock<IHubClients> HubClients = new();

    protected NotificacionService CreateService()
    {
        var options = new DbContextOptionsBuilder<BlogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Db = new BlogDbContext(options);

        // Configurar HubContext
        HubClients.Setup(c => c.User(It.IsAny<string>())).Returns(ClientProxy.Object);
        Hub.Setup(h => h.Clients).Returns(HubClients.Object);

        return new NotificacionService(Db, Repo.Object, Hub.Object);
    }
}
