using System.Linq.Expressions;
using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using BlogApi.Tests.Common;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BlogApi.Tests.Services.Posts;

/// <summary>
/// Tests para PostService - Método UpdateAsync
/// </summary>
public class PostServiceUpdateTests : PostServiceTestBase
{
    /*private readonly Mock<IPostRepository> _repo = new();
    private readonly Mock<ITagRepository> _tagRepo = new();
    private readonly Mock<ICategoriaRepository> _categoriaRepo = new();
    private readonly Mock<ISanitizerService> _sanitizer = new();
    private readonly Mock<INotificacionService> _notificaciones = new();*/

    private readonly PostService _service;

    public PostServiceUpdateTests()
    {
        _service = CreateService();
    }

    // ------------------------------------------------------------
    // 1. Post no encontrado
    // ------------------------------------------------------------
    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenPostNotFound()
    {
        Repo.Setup(r =>
                r.Query()
                    .Include(It.IsAny<string>())
                    .FirstOrDefaultAsync(
                        It.IsAny<System.Linq.Expressions.Expression<Func<Post, bool>>>()
                    )
            )
            .ReturnsAsync((Post?)null);

        var result = await _service.UpdateAsync(
            id: 1,
            post: new Post(),
            tagIds: new List<int> { 1 },
            usuarioId: 1,
            puedeEditarTodo: true
        );

        Assert.False(result);
    }

    // ------------------------------------------------------------
    // 2. Usuario sin permisos
    // ------------------------------------------------------------
    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenUserHasNoPermission()
    {
        var existing = new Post
        {
            Id = 1,
            UsuarioId = 99, // otro usuario
            Tags = new List<Tag>(),
        };

        Repo.Setup(r =>
                r.Query()
                    .Include(It.IsAny<string>())
                    .FirstOrDefaultAsync(
                        It.IsAny<System.Linq.Expressions.Expression<Func<Post, bool>>>()
                    )
            )
            .ReturnsAsync(existing);

        var result = await _service.UpdateAsync(
            id: 1,
            post: new Post { Titulo = "Nuevo", Contenido = "Contenido" },
            tagIds: new List<int> { 1 },
            usuarioId: 1,
            puedeEditarTodo: false
        );

        Assert.False(result);
    }

    // ------------------------------------------------------------
    // 3. Actualiza campos correctamente
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

        var updated = new Post
        {
            Titulo = "Nuevo Título",
            Contenido = "Nuevo Contenido",
            CategoriaId = 2,
        };

        var tags = new List<Tag> { new Tag { Id = 1 } };

        Repo.Setup(r =>
                r.Query()
                    .Include(It.IsAny<string>())
                    .FirstOrDefaultAsync(
                        It.IsAny<System.Linq.Expressions.Expression<Func<Post, bool>>>()
                    )
            )
            .ReturnsAsync(existing);

        TagRepo.Setup(r => r.Query()).Returns(tags.AsQueryable());
        CategoriaRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Categoria { Id = 2 });

        Repo.Setup(r => r.Update(existing));
        Repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        var result = await _service.UpdateAsync(
            id: 1,
            post: updated,
            tagIds: new List<int> { 1 },
            usuarioId: 10,
            puedeEditarTodo: true
        );

        Assert.True(result);
        Assert.Equal("Nuevo Título", existing.Titulo);
        Assert.Equal("Nuevo Contenido", existing.Contenido);
        Assert.Equal(2, existing.CategoriaId);
        Assert.Single(existing.Tags);
    }

    // ------------------------------------------------------------
    // 4. No debe cambiar UsuarioId
    // ------------------------------------------------------------
    [Fact]
    public async Task UpdateAsync_ShouldNotChangeUsuarioId()
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
            UsuarioId = 999, // intento de cambiarlo
        };

        Repo.Setup(r =>
                r.Query()
                    .Include(It.IsAny<string>())
                    .FirstOrDefaultAsync(
                        It.IsAny<System.Linq.Expressions.Expression<Func<Post, bool>>>()
                    )
            )
            .ReturnsAsync(existing);

        TagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        CategoriaRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Categoria());

        Repo.Setup(r => r.Update(existing));
        Repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        await _service.UpdateAsync(1, updated, new List<int>(), 10, true);

        Assert.Equal(10, existing.UsuarioId);
    }

    // ------------------------------------------------------------
    // 5. Sanitización aplicada
    // ------------------------------------------------------------
    [Fact]
    public async Task UpdateAsync_ShouldSanitizeContent()
    {
        var existing = new Post
        {
            Id = 1,
            UsuarioId = 10,
            Tags = new List<Tag>(),
        };

        var updated = new Post { Titulo = " Título ", Contenido = "<script>alert(1)</script>" };

        Sanitizer.Setup(s => s.SanitizePlainText(" Título ")).Returns("Título");
        Sanitizer
            .Setup(s => s.SanitizeMarkdown("<script>alert(1)</script>"))
            .Returns("contenido-limpio");
        Sanitizer.Setup(s => s.ContainsDangerousPattern(It.IsAny<string>())).Returns(false);
        Repo.Setup(r =>
                r.Query()
                    .Include(It.IsAny<string>())
                    .FirstOrDefaultAsync(
                        It.IsAny<System.Linq.Expressions.Expression<Func<Post, bool>>>()
                    )
            )
            .ReturnsAsync(existing);

        TagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        CategoriaRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Categoria());

        Repo.Setup(r => r.Update(existing));
        Repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _service.UpdateAsync(1, updated, new List<int>(), 10, true);

        Assert.Equal("Título", existing.Titulo);
        Assert.Equal("contenido-limpio", existing.Contenido);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnTrue_WhenUserHasPermission()
    {
        var existing = new Post
        {
            Id = 1,
            UsuarioId = 99, // otro usuario
            Tags = new List<Tag>(),
        };

        Repo.Setup(r =>
                r.Query()
                    .Include(It.IsAny<string>())
                    .FirstOrDefaultAsync(It.IsAny<Expression<Func<Post, bool>>>())
            )
            .ReturnsAsync(existing);

        CategoriaRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Categoria());
        TagRepo
            .Setup(r => r.GetByIdsAsync(new List<int> { 1 }))
            .ReturnsAsync(new List<Tag> { new Tag { Id = 1 } });

        Sanitizer.Setup(s => s.SanitizePlainText(It.IsAny<string>())).Returns("Titulo");
        Sanitizer.Setup(s => s.SanitizeMarkdown(It.IsAny<string>())).Returns("Contenido");
        Sanitizer.Setup(s => s.ContainsDangerousPattern(It.IsAny<string>())).Returns(false);

        Repo.Setup(r => r.Update(existing));
        Repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var updated = new Post
        {
            Titulo = "Titulo",
            Contenido = "Contenido",
            CategoriaId = 2,
        };

        var result = await _service.UpdateAsync(
            id: 1,
            post: updated,
            tagIds: new List<int> { 1 },
            usuarioId: 123,
            puedeEditarTodo: true // ADMIN
        );

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenCategoriaDoesNotExist()
    {
        var existing = new Post
        {
            Id = 1,
            UsuarioId = 10,
            Tags = new List<Tag>(),
        };

        Repo.Setup(r =>
                r.Query()
                    .Include(It.IsAny<string>())
                    .FirstOrDefaultAsync(It.IsAny<Expression<Func<Post, bool>>>())
            )
            .ReturnsAsync(existing);

        CategoriaRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Categoria?)null);

        var updated = new Post
        {
            Titulo = "Nuevo",
            Contenido = "Contenido",
            CategoriaId = 999,
        };

        var result = await _service.UpdateAsync(
            1,
            updated,
            new List<int>(),
            usuarioId: 10,
            puedeEditarTodo: true
        );

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenTagsDoNotExist()
    {
        var existing = new Post
        {
            Id = 1,
            UsuarioId = 10,
            Tags = new List<Tag>(),
        };

        Repo.Setup(r =>
                r.Query()
                    .Include(It.IsAny<string>())
                    .FirstOrDefaultAsync(It.IsAny<Expression<Func<Post, bool>>>())
            )
            .ReturnsAsync(existing);

        CategoriaRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Categoria());

        TagRepo.Setup(r => r.GetByIdsAsync(new List<int> { 1, 2 })).ReturnsAsync(new List<Tag>()); // vacío → no existen

        var updated = new Post
        {
            Titulo = "Nuevo",
            Contenido = "Contenido",
            CategoriaId = 1,
        };

        var result = await _service.UpdateAsync(
            1,
            updated,
            new List<int> { 1, 2 },
            usuarioId: 10,
            puedeEditarTodo: true
        );

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenContentIsDangerous()
    {
        var existing = new Post
        {
            Id = 1,
            UsuarioId = 10,
            Tags = new List<Tag>(),
        };

        Repo.Setup(r =>
                r.Query()
                    .Include(It.IsAny<string>())
                    .FirstOrDefaultAsync(It.IsAny<Expression<Func<Post, bool>>>())
            )
            .ReturnsAsync(existing);

        CategoriaRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Categoria());
        TagRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<int>>())).ReturnsAsync(new List<Tag>());

        Sanitizer.Setup(s => s.ContainsDangerousPattern(It.IsAny<string>())).Returns(true);

        var updated = new Post
        {
            Titulo = "Nuevo",
            Contenido = "<script>mal</script>",
            CategoriaId = 1,
        };

        var result = await _service.UpdateAsync(
            1,
            updated,
            new List<int>(),
            usuarioId: 10,
            puedeEditarTodo: true
        );

        Assert.False(result);
    }
}
