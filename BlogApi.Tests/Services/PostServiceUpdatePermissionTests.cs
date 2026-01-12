using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BlogApi.Tests.Services;

public class PostServiceUpdatePermissionTests
{
    private readonly Mock<IPostRepository> _repo = new();
    private readonly Mock<ITagRepository> _tagRepo = new();
    private readonly PostService _service;

    public PostServiceUpdatePermissionTests()
    {
        _service = new PostService(_repo.Object, _tagRepo.Object);
    }

    // Helper para simular Query().Include().FirstOrDefaultAsync()
    private void SetupExistingPost(Post? post)
    {
        var queryable = new List<Post> { post! }.AsQueryable();

        _repo.Setup(r => r.Query()).Returns(queryable);

        _repo.Setup(r => r.Query().Include(It.IsAny<string>())).Returns(queryable);

        _repo
            .Setup(r =>
                r.Query()
                    .FirstOrDefaultAsync(
                        It.IsAny<System.Linq.Expressions.Expression<Func<Post, bool>>>()
                    )
            )
            .ReturnsAsync(post);
    }

    // ------------------------------------------------------------
    // 1. No existe el post
    // ------------------------------------------------------------
    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenPostNotFound()
    {
        SetupExistingPost(null);

        var ok = await _service.UpdateAsync(1, new Post(), new List<int>(), true);

        Assert.False(ok);
    }

    // ------------------------------------------------------------
    // 2. Usuario NO es autor y NO tiene permisos
    // ------------------------------------------------------------
    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenUserIsNotAuthor_AndCannotEditAll()
    {
        var existing = new Post { Id = 1, UsuarioId = 10 };

        SetupExistingPost(existing);

        var updated = new Post { UsuarioId = 20 }; // otro usuario

        var ok = await _service.UpdateAsync(1, updated, new List<int>(), puedeEditarTodo: false);

        Assert.False(ok);
    }

    // ------------------------------------------------------------
    // 3. Usuario es el autor
    // ------------------------------------------------------------
    [Fact]
    public async Task UpdateAsync_ShouldReturnTrue_WhenUserIsAuthor()
    {
        var existing = new Post
        {
            Id = 1,
            UsuarioId = 10,
            Tags = new List<Tag>(),
        };

        SetupExistingPost(existing);

        var updated = new Post
        {
            UsuarioId = 10,
            Titulo = "Nuevo",
            Contenido = "Contenido",
        };

        _tagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var ok = await _service.UpdateAsync(1, updated, new List<int>(), puedeEditarTodo: false);

        Assert.True(ok);
    }

    // ------------------------------------------------------------
    // 4. Usuario NO es autor pero tiene permisos
    // ------------------------------------------------------------
    [Fact]
    public async Task UpdateAsync_ShouldReturnTrue_WhenUserIsNotAuthor_ButCanEditAll()
    {
        var existing = new Post
        {
            Id = 1,
            UsuarioId = 10,
            Tags = new List<Tag>(),
        };

        SetupExistingPost(existing);

        var updated = new Post
        {
            UsuarioId = 20,
            Titulo = "Nuevo",
            Contenido = "Contenido",
        };

        _tagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var ok = await _service.UpdateAsync(1, updated, new List<int>(), puedeEditarTodo: true);

        Assert.True(ok);
    }

    // ------------------------------------------------------------
    // 5. Actualiza campos correctamente
    // ------------------------------------------------------------
    [Fact]
    public async Task UpdateAsync_ShouldUpdateFieldsCorrectly()
    {
        var existing = new Post
        {
            Id = 1,
            UsuarioId = 10,
            Tags = new List<Tag>(),
        };

        SetupExistingPost(existing);

        var updated = new Post
        {
            UsuarioId = 10,
            Titulo = "Nuevo Título",
            Contenido = "Nuevo Contenido",
            CategoriaId = 3,
        };

        _tagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var ok = await _service.UpdateAsync(1, updated, new List<int>(), true);

        Assert.True(ok);
        Assert.Equal("Nuevo Título", existing.Titulo);
        Assert.Equal("Nuevo Contenido", existing.Contenido);
        Assert.Equal(3, existing.CategoriaId);
    }

    // ------------------------------------------------------------
    // 6. Actualiza tags correctamente
    // ------------------------------------------------------------
    [Fact]
    public async Task UpdateAsync_ShouldReplaceTags()
    {
        var existing = new Post
        {
            Id = 1,
            UsuarioId = 10,
            Tags = new List<Tag> { new Tag { Id = 1 } },
        };

        SetupExistingPost(existing);

        var updated = new Post { UsuarioId = 10 };

        var newTags = new List<Tag>
        {
            new Tag { Id = 2 },
            new Tag { Id = 3 },
        };

        _tagRepo.Setup(r => r.Query()).Returns(newTags.AsQueryable());
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var ok = await _service.UpdateAsync(1, updated, new List<int> { 2, 3 }, true);

        Assert.True(ok);
        Assert.Equal(2, existing.Tags.Count);
        Assert.Contains(existing.Tags, t => t.Id == 2);
        Assert.Contains(existing.Tags, t => t.Id == 3);
    }

    // ------------------------------------------------------------
    // 7. Llama a SaveChangesAsync
    // ------------------------------------------------------------
    [Fact]
    public async Task UpdateAsync_ShouldCallSaveChanges()
    {
        var existing = new Post
        {
            Id = 1,
            UsuarioId = 10,
            Tags = new List<Tag>(),
        };

        SetupExistingPost(existing);

        var updated = new Post { UsuarioId = 10 };

        _tagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _service.UpdateAsync(1, updated, new List<int>(), true);

        _repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
