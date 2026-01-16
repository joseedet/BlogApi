using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Tests.Common;
using Moq;

namespace BlogApi.Tests.Services.Posts;

public class PostServiceGetByIdTests : PostServiceTestBase
{
    //private readonly Mock<IPostRepository> _repo = new();
    //private readonly Mock<ITagRepository> _tagRepo = new();
    private readonly PostService _service;

    public PostServiceGetByIdTests()
    {
        _service = CreateService();
    }

    // ------------------------------------------------------------
    // GET BY ID
    // ------------------------------------------------------------
    [Fact]
    public async Task GetByIdAsync_ShouldReturnPost_WhenExists()
    {
        var post = new Post
        {
            Id = 1,
            Titulo = "Hola Mundo",
            Slug = "hola-mundo",
        };

        Repo.Setup(r => r.GetPostCompletoAsync(1)).ReturnsAsync(post);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal("Hola Mundo", result.Titulo);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        Repo.Setup(r => r.GetPostCompletoAsync(99)).ReturnsAsync((Post?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldCallRepositoryWithCorrectId()
    {
        Repo.Setup(r => r.GetPostCompletoAsync(It.IsAny<int>())).ReturnsAsync((Post?)null);

        await _service.GetByIdAsync(5);

        Repo.Verify(r => r.GetPostCompletoAsync(5), Times.Once);
    }
}
