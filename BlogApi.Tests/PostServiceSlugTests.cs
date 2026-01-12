using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services;
using Moq;
using Xunit;

namespace BlogApi.Tests;

public class PostServiceSlugTests
{
    private readonly Mock<IPostRepository> _repo = new();
    private readonly Mock<ITagRepository> _tagRepo = new();
    private readonly PostService _service;

    public PostServiceSlugTests()
    {
        _service = new PostService(_repo.Object, _tagRepo.Object);
    }

    // ------------------------------------------------------------
    // GET BY SLUG
    // ------------------------------------------------------------
    [Fact]
    public async Task GetBySlugAsync_ShouldReturnPost_WhenExists()
    {
        var post = new Post
        {
            Id = 1,
            Slug = "hola-mundo",
            Titulo = "Hola Mundo",
        };

        _repo.Setup(r => r.GetBySlugAsync("hola-mundo")).ReturnsAsync(post);

        var result = await _service.GetBySlugAsync("hola-mundo");

        Assert.NotNull(result);
        Assert.Equal("hola-mundo", result!.Slug);
        Assert.Equal("Hola Mundo", result.Titulo);
    }

    [Fact]
    public async Task GetBySlugAsync_ShouldReturnNull_WhenNotFound()
    {
        _repo.Setup(r => r.GetBySlugAsync("no-existe")).ReturnsAsync((Post?)null);

        var result = await _service.GetBySlugAsync("no-existe");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetBySlugAsync_ShouldCallRepositoryWithCorrectSlug()
    {
        _repo.Setup(r => r.GetBySlugAsync(It.IsAny<string>())).ReturnsAsync((Post?)null);

        await _service.GetBySlugAsync("mi-slug");

        _repo.Verify(r => r.GetBySlugAsync("mi-slug"), Times.Once);
    }
}
