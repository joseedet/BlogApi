using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services;
using Moq;

namespace BlogApi.Tests;

public class PostServiceGetByTagTests
{
    private readonly Mock<IPostRepository> _repo = new();
    private readonly Mock<ITagRepository> _tagRepo = new();
    private readonly PostService _service;

    public PostServiceGetByTagTests()
    {
        _service = new PostService(_repo.Object, _tagRepo.Object);
    }

    // ------------------------------------------------------------
    // GET BY TAG ID
    // ------------------------------------------------------------
    [Fact]
    public async Task GetByTagAsync_ShouldReturnPostsForTag()
    {
        var posts = new List<Post>
        {
            new Post
            {
                Id = 1,
                Tags = new List<Tag> { new Tag { Id = 5 } },
            },
            new Post
            {
                Id = 2,
                Tags = new List<Tag> { new Tag { Id = 5 } },
            },
        };

        _repo.Setup(r => r.GetByTagAsync(5)).ReturnsAsync(posts);

        var result = await _service.GetByTagAsync(5);

        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Contains(p.Tags, t => t.Id == 5));
    }

    [Fact]
    public async Task GetByTagAsync_ShouldReturnEmpty_WhenNoMatches()
    {
        _repo.Setup(r => r.GetByTagAsync(99)).ReturnsAsync(new List<Post>());

        var result = await _service.GetByTagAsync(99);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByTagAsync_ShouldCallRepositoryWithCorrectId()
    {
        _repo.Setup(r => r.GetByTagAsync(It.IsAny<int>())).ReturnsAsync(new List<Post>());

        await _service.GetByTagAsync(7);

        _repo.Verify(r => r.GetByTagAsync(7), Times.Once);
    }
}
