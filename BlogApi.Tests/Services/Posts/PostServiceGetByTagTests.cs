using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Tests.Common;
using Moq;

namespace BlogApi.Tests.Services.Posts;

public class PostServiceGetByTagTests : PostServiceTestBase
{
    private readonly PostService _service;

    public PostServiceGetByTagTests()
    {
        _service = CreateService();
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

        Repo.Setup(r => r.GetByTagAsync(5)).ReturnsAsync(posts);

        var result = await _service.GetByTagAsync(5);

        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Contains(p.Tags, t => t.Id == 5));
    }

    [Fact]
    public async Task GetByTagAsync_ShouldReturnEmpty_WhenNoMatches()
    {
        Repo.Setup(r => r.GetByTagAsync(99)).ReturnsAsync(new List<Post>());

        var result = await _service.GetByTagAsync(99);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByTagAsync_ShouldCallRepositoryWithCorrectId()
    {
        Repo.Setup(r => r.GetByTagAsync(It.IsAny<int>())).ReturnsAsync(new List<Post>());

        await _service.GetByTagAsync(7);

        Repo.Verify(r => r.GetByTagAsync(7), Times.Once);
    }
}
