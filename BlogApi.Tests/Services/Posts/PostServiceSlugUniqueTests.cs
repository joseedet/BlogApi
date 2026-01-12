using System.Linq;
using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BlogApi.Tests.Services.Posts;

public class PostServiceSlugUniqueTests
{
    private readonly Mock<IPostRepository> _repo = new();
    private readonly Mock<ITagRepository> _tagRepo = new();
    private readonly PostService _service;

    public PostServiceSlugUniqueTests()
    {
        _service = new PostService(_repo.Object, _tagRepo.Object);
    }

    // Helper para simular Query().AnyAsync()
    private void SetupSlugExistsSequence(params bool[] existsSequence)
    {
        var queue = new Queue<bool>(existsSequence);

        _repo
            .Setup(r =>
                r.Query().AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Post, bool>>>())
            )
            .ReturnsAsync(() => queue.Dequeue());
    }

    // ------------------------------------------------------------
    // 1. Slug base normal
    // ------------------------------------------------------------
    [Fact]
    public async Task CreateAsync_ShouldGenerateBaseSlug_WhenNoConflicts()
    {
        var post = new Post { Titulo = "Hola Mundo" };

        SetupSlugExistsSequence(false); // No existe ningún slug igual

        _tagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        _repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(post, new List<int>());

        Assert.Equal("hola-mundo", result.Slug);
    }

    // ------------------------------------------------------------
    // 2. Slug repetido una vez
    // ------------------------------------------------------------
    [Fact]
    public async Task CreateAsync_ShouldAppendCounter_WhenSlugExistsOnce()
    {
        var post = new Post { Titulo = "Hola Mundo" };

        SetupSlugExistsSequence(true, false);
        // 1) "hola-mundo" existe
        // 2) "hola-mundo-1" NO existe

        _tagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        _repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(post, new List<int>());

        Assert.Equal("hola-mundo-1", result.Slug);
    }

    // ------------------------------------------------------------
    // 3. Slug repetido varias veces
    // ------------------------------------------------------------
    [Fact]
    public async Task CreateAsync_ShouldIncrementCounterUntilUnique()
    {
        var post = new Post { Titulo = "Hola Mundo" };

        SetupSlugExistsSequence(true, true, true, false);
        // "hola-mundo" existe
        // "hola-mundo-1" existe
        // "hola-mundo-2" existe
        // "hola-mundo-3" NO existe

        _tagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        _repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(post, new List<int>());

        Assert.Equal("hola-mundo-3", result.Slug);
    }

    // ------------------------------------------------------------
    // 4. Slug con caracteres especiales
    // ------------------------------------------------------------
    [Fact]
    public async Task CreateAsync_ShouldNormalizeSpecialCharacters()
    {
        var post = new Post { Titulo = "Hola @ Mundo!!!" };

        SetupSlugExistsSequence(false);

        _tagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        _repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(post, new List<int>());

        Assert.Equal("hola-mundo", result.Slug);
    }

    // ------------------------------------------------------------
    // 5. Slug con espacios múltiples
    // ------------------------------------------------------------
    [Fact]
    public async Task CreateAsync_ShouldNormalizeMultipleSpaces()
    {
        var post = new Post { Titulo = "Hola     Mundo   Nuevo" };

        SetupSlugExistsSequence(false);

        _tagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        _repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(post, new List<int>());

        Assert.Equal("hola-mundo-nuevo", result.Slug);
    }

    // ------------------------------------------------------------
    // 6. Slug con mayúsculas
    // ------------------------------------------------------------
    [Fact]
    public async Task CreateAsync_ShouldLowercaseSlug()
    {
        var post = new Post { Titulo = "HOLA MUNDO" };

        SetupSlugExistsSequence(false);

        _tagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        _repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(post, new List<int>());

        Assert.Equal("hola-mundo", result.Slug);
    }

    // ------------------------------------------------------------
    // 7. Slug con acentos
    // ------------------------------------------------------------
    [Fact]
    public async Task CreateAsync_ShouldRemoveAccents()
    {
        var post = new Post { Titulo = "Canción Única" };

        SetupSlugExistsSequence(false);

        _tagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        _repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(post, new List<int>());

        Assert.Equal("cancion-unica", result.Slug);
    }

    // ------------------------------------------------------------
    // 8. Verificar llamadas al repositorio
    // ------------------------------------------------------------
    [Fact]
    public async Task CreateAsync_ShouldCallRepositoryUntilSlugIsUnique()
    {
        var post = new Post { Titulo = "Hola Mundo" };

        SetupSlugExistsSequence(true, true, false);

        _tagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        _repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _service.CreateAsync(post, new List<int>());

        // Debe llamar 3 veces a AnyAsync()
        _repo.Verify(
            r =>
                r.Query()
                    .AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Post, bool>>>()),
            Times.Exactly(3)
        );
    }
}
