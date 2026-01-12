using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services;
using Moq;

namespace BlogApi.Tests;

public class PostServiceSearchPagedTests
{
    private readonly Mock<IPostRepository> _repo = new();
    private readonly Mock<ITagRepository> _tagRepo = new();
    private readonly PostService _service;

    public PostServiceSearchPagedTests()
    {
        _service = new PostService(_repo.Object, _tagRepo.Object);
    }

    // ------------------------------------------------------------
    // SEARCH PAGED
    // ------------------------------------------------------------
    [Fact]
    public async Task SearchPagedAsync_ShouldReturnPagedResults()
    {
        var posts = new List<Post>
        {
            new Post { Id = 1, Titulo = "Hola mundo" },
            new Post { Id = 2, Titulo = "Hola backend" },
        };

        var paged = new PaginationDto<Post>
        {
            Pagina = 1,
            Tamano = 10,
            Total = 2,
            Items = posts,
        };

        _repo.Setup(r => r.SearchPagedAsync("hola", 1, 10)).ReturnsAsync(paged);

        var result = await _service.SearchPagedAsync("hola", 1, 10);

        Assert.Equal(1, result.Pagina);
        Assert.Equal(10, result.Tamano);
        Assert.Equal(2, result.Total);
        Assert.Equal(2, result.Items.Count());
    }

    [Fact]
    public async Task SearchPagedAsync_ShouldReturnEmpty_WhenNoMatches()
    {
        var paged = new PaginationDto<Post>
        {
            Pagina = 1,
            Tamano = 10,
            Total = 0,
            Items = new List<Post>(),
        };

        _repo.Setup(r => r.SearchPagedAsync("no-existe", 1, 10)).ReturnsAsync(paged);

        var result = await _service.SearchPagedAsync("no-existe", 1, 10);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task SearchPagedAsync_ShouldCallRepositoryWithCorrectParameters()
    {
        _repo
            .Setup(r => r.SearchPagedAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(
                new PaginationDto<Post>
                {
                    Pagina = 1,
                    Tamano = 10,
                    Total = 0,
                    Items = new List<Post>(),
                }
            );

        await _service.SearchPagedAsync("backend", 2, 5);

        _repo.Verify(r => r.SearchPagedAsync("backend", 2, 5), Times.Once);
    }
}
