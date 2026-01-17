using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Tests.Common;

namespace BlogApi.Tests.Services.Posts;

public class PostServiceAutorTests : PostServiceTestBase
{
    private readonly PostService _service;

    public PostServiceAutorTests()
    {
        _service = CreateService();
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
        }.AsQueryable();

        Repo.Setup(r => r.Query()).Returns(posts);

        var result = await _service.GetByAutorAsync(10);

        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Equal(10, p.UsuarioId));
    }

    [Fact]
    public async Task GetByAutorAsync_ShouldReturnEmpty_WhenNoMatches()
    {
        var posts = new List<Post>().AsQueryable();

        Repo.Setup(r => r.Query()).Returns(posts);

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
        }.AsQueryable();

        Repo.Setup(r => r.Query()).Returns(posts);

        var result = await _service.GetByAutorNombreAsync("jose");

        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Contains("jose", p.Usuario.Nombre.ToLower()));
    }

    [Fact]
    public async Task GetByAutorNombreAsync_ShouldReturnEmpty_WhenNoMatches()
    {
        var posts = new List<Post>().AsQueryable();

        Repo.Setup(r => r.Query()).Returns(posts);

        var result = await _service.GetByAutorNombreAsync("no-existe");

        Assert.Empty(result);
    }
}
