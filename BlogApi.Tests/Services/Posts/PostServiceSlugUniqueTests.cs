using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Tests.Common;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BlogApi.Tests.Services.Posts;

public class PostServiceSlugUniqueTests : PostServiceTestBase
{
    private readonly PostService _service;

    public PostServiceSlugUniqueTests()
    {
        _service = CreateService();
    }

    // Helper para simular Query().AnyAsync()
    private void SetupSlugExistsSequence(params bool[] existsSequence)
    {
        var queue = new Queue<bool>(existsSequence);

        Repo.Setup(r =>
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

        TagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        Repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        Repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(post, new List<int>(), 123);

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

        TagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        Repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        Repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(post, new List<int>(), 123);

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

        TagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        Repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        Repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(post, new List<int>(), 123);

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

        TagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        Repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        Repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(post, new List<int>(), 123);

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

        TagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        Repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        Repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(post, new List<int>(), 123);

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

        TagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        Repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        Repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(post, new List<int>(), 123);

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

        TagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        Repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        Repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(post, new List<int>(), 123);

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

        TagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        Repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        Repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _service.CreateAsync(post, new List<int>(), 123);

        // Debe llamar 3 veces a AnyAsync()
        Repo.Verify(
            r =>
                r.Query()
                    .AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Post, bool>>>()),
            Times.Exactly(3)
        );
    }
}
