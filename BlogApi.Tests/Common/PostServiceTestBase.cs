using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using Moq;

namespace BlogApi.Tests.Common;

public abstract class PostServiceTestBase
{
    protected readonly Mock<IPostRepository> Repo = new();
    protected readonly Mock<ITagRepository> TagRepo = new();
    protected readonly Mock<ICategoriaRepository> CategoriaRepo = new();
    protected readonly Mock<ISanitizerService> Sanitizer = new();
    protected readonly Mock<INotificacionService> Notificaciones = new();

    protected PostService CreateService()
    {
        return new PostService(
            Repo.Object,
            TagRepo.Object,
            CategoriaRepo.Object,
            Sanitizer.Object,
            Notificaciones.Object
        );
    }
}
