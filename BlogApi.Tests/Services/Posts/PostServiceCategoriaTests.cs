using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using Moq;

namespace BlogApi.Tests.Services.Posts;

public class PostServiceCategoriaTests
{
    private readonly Mock<IPostRepository> _repo = new();
    private readonly Mock<ITagRepository> _tagRepo = new();
    private readonly Mock<ICategoriaRepository> _categoriaRepo = new();
    private readonly Mock<ISanitizerService> _sanitizer = new();
    private readonly Mock<INotificacionService> _notificaciones = new();
    private readonly PostService _service;

    public PostServiceCategoriaTests()
    {
        _service = new PostService(
            _repo.Object,
            _tagRepo.Object,
            _categoriaRepo.Object,
            _sanitizer.Object,
            _notificaciones.Object
        );
    }

    // ------------------------------------------------------------
    // GET BY CATEGORIA ID
    // ------------------------------------------------------------
    [Fact]
    public async Task GetByCategoriaAsync_ShouldReturnPostsForCategoria()
    {
        var posts = new List<Post>
        {
            new Post { Id = 1, CategoriaId = 10 },
            new Post { Id = 2, CategoriaId = 10 },
        };

        _repo.Setup(r => r.GetByCategoriaAsync(10)).ReturnsAsync(posts);

        var result = await _service.GetByCategoriaAsync(10);

        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Equal(10, p.CategoriaId));
    }

    [Fact]
    public async Task GetByCategoriaAsync_ShouldReturnEmpty_WhenNoMatches()
    {
        _repo.Setup(r => r.GetByCategoriaAsync(99)).ReturnsAsync(new List<Post>());

        var result = await _service.GetByCategoriaAsync(99);

        Assert.Empty(result);
    }

    // ------------------------------------------------------------
    // GET BY CATEGORIA SLUG
    // ------------------------------------------------------------
    [Fact]
    public async Task GetByCategoriaSlugAsync_ShouldReturnPostsForSlug()
    {
        var posts = new List<Post>
        {
            new Post
            {
                Id = 1,
                Categoria = new Categoria { Slug = "backend" },
            },
            new Post
            {
                Id = 2,
                Categoria = new Categoria { Slug = "backend" },
            },
        };

        _repo.Setup(r => r.GetByCategoriaSlugAsync("backend")).ReturnsAsync(posts);

        var result = await _service.GetByCategoriaSlugAsync("backend");

        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Equal("backend", p.Categoria.Slug));
    }

    [Fact]
    public async Task GetByCategoriaSlugAsync_ShouldReturnEmpty_WhenNoMatches()
    {
        _repo.Setup(r => r.GetByCategoriaSlugAsync("no-existe")).ReturnsAsync(new List<Post>());

        var result = await _service.GetByCategoriaSlugAsync("no-existe");

        Assert.Empty(result);
    }
}
