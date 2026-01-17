using BlogApi.Data;
using BlogApi.Hubs;
using BlogApi.Repositories;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BlogApi.Tests.Common;

public abstract class NotificacionServiceTestBase
{
    protected readonly Mock<INotificacionRepository> Repo = new();
    protected readonly Mock<IHubContext<NotificacionesHub>> Hub = new();

    protected BlogDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<BlogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new BlogDbContext(options);
    }

    protected NotificacionService CreateService()
    {
        var db = CreateDbContext();

        return new NotificacionService(db, Repo.Object, Hub.Object);
    }
}
