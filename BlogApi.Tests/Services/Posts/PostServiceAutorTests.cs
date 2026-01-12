using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services;
using Moq;
using Xunit;

namespace BlogApi.Tests.Services.Posts;

public class PostServiceAutorTests
{
    private readonly Mock<IPostRepository> _repo = new();
    private readonly Mock<ITagRepository> _tagRepo = new();
    private readonly PostService _service;

    public PostServiceAutorTests()
    {
        _service = new PostService(_repo.Object, _tagRepo.Object);
    }

    // ------------------------------------------------------------
    // GET BY AUTOR ID
    // ------------------------------------------------------------
    [Fact]
    public async Task GetByAutorAsync_ShouldReturnPostsForUser()
    {
        var posts = new List<Post>
        {
            new Post { Id = 1, UsuarioId = 10 },
            new Post { Id = 2, UsuarioId = 10 },
        };

        _repo.Setup(r => r.GetByAutorAsync(10)).ReturnsAsync(posts);

        var result = await _service.GetByAutorAsync(10);

        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Equal(10, p.UsuarioId));
    }

    [Fact]
    public async Task GetByAutorAsync_ShouldReturnEmpty_WhenNoMatches()
    {
        _repo.Setup(r => r.GetByAutorAsync(99)).ReturnsAsync(new List<Post>());

        var result = await _service.GetByAutorAsync(99);

        Assert.Empty(result);
    }

    // ------------------------------------------------------------
    // GET BY AUTOR NOMBRE
    // ------------------------------------------------------------
    [Fact]
    public async Task GetByAutorNombreAsync_ShouldReturnMatches()
    {
        var posts = new List<Post>
        {
            new Post
            {
                Id = 1,
                Usuario = new Usuario { Nombre = "Jose" },
            },
            new Post
            {
                Id = 2,
                Usuario = new Usuario { Nombre = "Jose Antonio" },
            },
        };

        _repo.Setup(r => r.GetByAutorNombreAsync("jose")).ReturnsAsync(posts);

        var result = await _service.GetByAutorNombreAsync("jose");

        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Contains("jose", p.Usuario.Nombre.ToLower()));
    }

    [Fact]
    public async Task GetByAutorNombreAsync_ShouldReturnEmpty_WhenNoMatches()
    {
        _repo.Setup(r => r.GetByAutorNombreAsync("no-existe")).ReturnsAsync(new List<Post>());

        var result = await _service.GetByAutorNombreAsync("no-existe");

        Assert.Empty(result);
    }
}
