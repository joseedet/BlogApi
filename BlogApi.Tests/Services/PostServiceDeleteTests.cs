using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services;
using Moq;
using Xunit;

namespace BlogApi.Tests.Services;

public class PostServiceDeleteTests
{
    private readonly Mock<IPostRepository> _repo = new();
    private readonly Mock<ITagRepository> _tagRepo = new();
    private readonly PostService _service;

    public PostServiceDeleteTests()
    {
        _service = new PostService(_repo.Object, _tagRepo.Object);
    }

    // ------------------------------------------------------------
    // 1. Post no encontrado
    // ------------------------------------------------------------
    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenPostNotFound()
    {
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Post?)null);

        var result = await _service.DeleteAsync(1);

        Assert.False(result);
        _repo.Verify(r => r.Remove(It.IsAny<Post>()), Times.Never);
        _repo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    // ------------------------------------------------------------
    // 2. Post encontrado
    // ------------------------------------------------------------
    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenPostExists()
    {
        var post = new Post { Id = 1 };

        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
    }

    // ------------------------------------------------------------
    // 3. Debe llamar a Remove con el post correcto
    // ------------------------------------------------------------
    [Fact]
    public async Task DeleteAsync_ShouldCallRemoveWithCorrectPost()
    {
        var post = new Post { Id = 1 };

        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _service.DeleteAsync(1);

        _repo.Verify(r => r.Remove(post), Times.Once);
    }

    // ------------------------------------------------------------
    // 4. Debe llamar a SaveChangesAsync una vez
    // ------------------------------------------------------------
    [Fact]
    public async Task DeleteAsync_ShouldCallSaveChangesOnce()
    {
        var post = new Post { Id = 1 };

        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _service.DeleteAsync(1);

        _repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
