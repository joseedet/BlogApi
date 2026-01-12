using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BlogApi.Tests.Services;

public class PostServiceTests
{
    private readonly Mock<IPostRepository> _repo = new();
    private readonly Mock<ITagRepository> _tagRepo = new();
    private readonly PostService _service;

    public PostServiceTests()
    {
        _service = new PostService(_repo.Object, _tagRepo.Object);
    }

    // ------------------------------------------------------------
    // CREATE
    // ------------------------------------------------------------
    [Fact]
    public async Task CreateAsync_ShouldGenerateUniqueSlug()
    {
        var post = new Post { Titulo = "Hola Mundo" };

        _repo
            .Setup(r =>
                r.Query().AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Post, bool>>>())
            )
            .ReturnsAsync(false);

        _tagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());

        _repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(post, new List<int>());

        Assert.Equal("hola-mundo", result.Slug);
    }

    [Fact]
    public async Task CreateAsync_ShouldAssignTags()
    {
        var post = new Post { Titulo = "Test" };

        var tags = new List<Tag>
        {
            new Tag { Id = 1, Nombre = "C#" },
            new Tag { Id = 2, Nombre = "Backend" },
        };

        _tagRepo.Setup(r => r.Query()).Returns(tags.AsQueryable());

        _repo
            .Setup(r =>
                r.Query().AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Post, bool>>>())
            )
            .ReturnsAsync(false);

        _repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(post, new List<int> { 1, 2 });

        Assert.Equal(2, result.Tags.Count);
    }

    // ------------------------------------------------------------
    // UPDATE
    // ------------------------------------------------------------
    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenPostNotFound()
    {
        _repo
            .Setup(r =>
                r.Query()
                    .FirstOrDefaultAsync(
                        It.IsAny<System.Linq.Expressions.Expression<Func<Post, bool>>>()
                    )
            )
            .ReturnsAsync((Post?)null);

        var ok = await _service.UpdateAsync(1, new Post(), new List<int>(), true);

        Assert.False(ok);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateFields()
    {
        var existing = new Post
        {
            Id = 1,
            UsuarioId = 10,
            Tags = new List<Tag>(),
        };
        var updated = new Post
        {
            Titulo = "Nuevo",
            Contenido = "Contenido",
            UsuarioId = 10,
        };

        var tags = new List<Tag> { new Tag { Id = 1 } };

        _repo.Setup(r => r.Query().Include(It.IsAny<string>())).Returns(_repo.Object.Query());
        _repo
            .Setup(r =>
                r.Query()
                    .FirstOrDefaultAsync(
                        It.IsAny<System.Linq.Expressions.Expression<Func<Post, bool>>>()
                    )
            )
            .ReturnsAsync(existing);

        _tagRepo.Setup(r => r.Query()).Returns(tags.AsQueryable());

        _repo.Setup(r => r.Update(existing));
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var ok = await _service.UpdateAsync(1, updated, new List<int> { 1 }, true);

        Assert.True(ok);
        Assert.Equal("Nuevo", existing.Titulo);
        Assert.Equal("Contenido", existing.Contenido);
        Assert.Single(existing.Tags);
    }

    // ------------------------------------------------------------
    // DELETE
    // ------------------------------------------------------------
    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
    {
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Post?)null);

        var ok = await _service.DeleteAsync(1);

        Assert.False(ok);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemovePost()
    {
        var post = new Post { Id = 1 };

        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var ok = await _service.DeleteAsync(1);

        Assert.True(ok);
        _repo.Verify(r => r.Remove(post), Times.Once);
    }

    // ------------------------------------------------------------
    // PAGINACIÓN
    // ------------------------------------------------------------
    [Fact]
    public async Task GetPagedAsync_ShouldReturnCorrectData()
    {
        var posts = Enumerable.Range(1, 10).Select(i => new Post { Id = i }).ToList();

        _repo
            .Setup(r => r.GetPagedAsync(1, 10))
            .ReturnsAsync(
                new DTO.PaginationDto<Post>
                {
                    Pagina = 1,
                    Tamano = 10,
                    Total = 10,
                    Items = posts,
                }
            );

        var result = await _service.GetPagedAsync(1, 10);

        Assert.Equal(10, result.Items.Count());
    }

    // ------------------------------------------------------------
    // CURSOR PAGINATION
    // ------------------------------------------------------------
    [Fact]
    public async Task GetCursorPagedAsync_ShouldReturnNextCursor()
    {
        var posts = new List<Post>
        {
            new Post { Id = 5 },
            new Post { Id = 6 },
        };

        _repo
            .Setup(r => r.GetCursorPagedAsync(5, 2))
            .ReturnsAsync(new DTO.CursorPaginationDto<Post> { Items = posts, NextCursor = 6 });

        var result = await _service.GetCursorPagedAsync(5, 2);

        Assert.Equal(6, result.NextCursor);
    }

    // ------------------------------------------------------------
    // SEARCH
    // ------------------------------------------------------------
    [Fact]
    public async Task SearchAsync_ShouldReturnMatches()
    {
        var posts = new List<Post> { new Post { Titulo = "Hola mundo" } };

        _repo.Setup(r => r.SearchAsync("hola")).ReturnsAsync(posts);

        var result = await _service.SearchAsync("hola");

        Assert.Single(result);
    }
}
