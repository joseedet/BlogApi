using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services;
using BlogApi.Tests.Common;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BlogApi.Testss.Services.Posts;

/// <summary>
/// Tests para el método GetAllAsync() del PostService
/// </summary>
public class PostServiceGetAllTests : PostServiceTestBase
{
    //private readonly Mock<IPostRepository> _repo = new();
    //private readonly Mock<ITagRepository> _tagRepo = new();
    private readonly PostService _service;

    public PostServiceGetAllTests()
    {
        _service = CreateService();
    }

    // ------------------------------------------------------------
    // GET ALL
    // ------------------------------------------------------------
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPosts()
    {
        var posts = new List<Post>
        {
            new Post { Id = 1, Titulo = "Post 1" },
            new Post { Id = 2, Titulo = "Post 2" },
        };

        // Simulamos Query() devolviendo un IQueryable
        var queryable = posts.AsQueryable();

        Repo.Setup(r => r.Query()).Returns(queryable);

        // Simulamos ToListAsync()
        Repo.Setup(r => r.Query().ToListAsync(default)).ReturnsAsync(posts);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count());
        Assert.Contains(result, p => p.Id == 1);
        Assert.Contains(result, p => p.Id == 2);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmpty_WhenNoPosts()
    {
        var posts = new List<Post>();

        var queryable = posts.AsQueryable();

        Repo.Setup(r => r.Query()).Returns(queryable);

        Repo.Setup(r => r.Query().ToListAsync(default)).ReturnsAsync(posts);

        var result = await _service.GetAllAsync();

        Assert.Empty(result);
    }
}
