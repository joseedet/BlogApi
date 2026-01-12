using System.Linq;
using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BlogApi.Tests;

public class PostServiceGetAllAsyncTests
{
    private readonly Mock<IPostRepository> _repo = new();
    private readonly Mock<ITagRepository> _tagRepo = new();
    private readonly PostService _service;

    public PostServiceGetAllAsyncTests()
    {
        _service = new PostService(_repo.Object, _tagRepo.Object);
    }

    // Helper para simular Query() + ToListAsync()
    private void SetupQueryable(List<Post> posts)
    {
        var queryable = posts.AsQueryable();

        // Query() devuelve el IQueryable simulado
        _repo.Setup(r => r.Query()).Returns(queryable);

        // ToListAsync() devuelve la lista simulada
        _repo.Setup(r => r.Query().ToListAsync(default)).ReturnsAsync(posts);
    }

    // ------------------------------------------------------------
    // 1. Devuelve todos los posts
    // ------------------------------------------------------------
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPosts()
    {
        var posts = new List<Post>
        {
            new Post { Id = 1, Titulo = "Post 1" },
            new Post { Id = 2, Titulo = "Post 2" },
        };

        SetupQueryable(posts);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count());
        Assert.Contains(result, p => p.Id == 1);
        Assert.Contains(result, p => p.Id == 2);
    }

    // ------------------------------------------------------------
    // 2. Devuelve lista vacía
    // ------------------------------------------------------------
    [Fact]
    public async Task GetAllAsync_ShouldReturnEmpty_WhenNoPosts()
    {
        var posts = new List<Post>();

        SetupQueryable(posts);

        var result = await _service.GetAllAsync();

        Assert.Empty(result);
    }

    // ------------------------------------------------------------
    // 3. Verifica que se llama a Query() una vez
    // ------------------------------------------------------------
    [Fact]
    public async Task GetAllAsync_ShouldCallQueryOnce()
    {
        var posts = new List<Post>();

        SetupQueryable(posts);

        await _service.GetAllAsync();

        _repo.Verify(r => r.Query(), Times.Once);
    }

    // ------------------------------------------------------------
    // 4. Verifica que se llama a ToListAsync()
    // ------------------------------------------------------------
    [Fact]
    public async Task GetAllAsync_ShouldCallToListAsync()
    {
        var posts = new List<Post>();

        SetupQueryable(posts);

        await _service.GetAllAsync();

        _repo.Verify(r => r.Query().ToListAsync(default), Times.Once);
    }
}
