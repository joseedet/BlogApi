using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BlogApi.Tests.Services.Posts;

public class PostServiceUpdateTests
{
    private readonly Mock<IPostRepository> _repo = new();
    private readonly Mock<ITagRepository> _tagRepo = new();
    private readonly Mock<ICategoriaRepository> _categoriaRepo = new();
    private readonly Mock<ISanitizerService> _sanitizer = new();
    private readonly Mock<INotificacionService> _notificaciones = new();

    private readonly PostService _service;

    public PostServiceUpdateTests()
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
    // 1. Post no encontrado
    // ------------------------------------------------------------
    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenPostNotFound()
    {
        _repo
            .Setup(r =>
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

        _repo
            .Setup(r =>
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

        _repo
            .Setup(r =>
                r.Query()
                    .Include(It.IsAny<string>())
                    .FirstOrDefaultAsync(
                        It.IsAny<System.Linq.Expressions.Expression<Func<Post, bool>>>()
                    )
            )
            .ReturnsAsync(existing);

        _tagRepo.Setup(r => r.Query()).Returns(tags.AsQueryable());
        _categoriaRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Categoria { Id = 2 });

        _repo.Setup(r => r.Update(existing));
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

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

        _repo
            .Setup(r =>
                r.Query()
                    .Include(It.IsAny<string>())
                    .FirstOrDefaultAsync(
                        It.IsAny<System.Linq.Expressions.Expression<Func<Post, bool>>>()
                    )
            )
            .ReturnsAsync(existing);

        _tagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        _categoriaRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Categoria());

        _repo.Setup(r => r.Update(existing));
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

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

        _sanitizer.Setup(s => s.SanitizePlainText(" Título ")).Returns("Título");
        _sanitizer
            .Setup(s => s.SanitizeMarkdown("<script>alert(1)</script>"))
            .Returns("contenido-limpio");
        _sanitizer.Setup(s => s.ContainsDangerousPattern(It.IsAny<string>())).Returns(false);

        _repo
            .Setup(r =>
                r.Query()
                    .Include(It.IsAny<string>())
                    .FirstOrDefaultAsync(
                        It.IsAny<System.Linq.Expressions.Expression<Func<Post, bool>>>()
                    )
            )
            .ReturnsAsync(existing);

        _tagRepo.Setup(r => r.Query()).Returns(new List<Tag>().AsQueryable());
        _categoriaRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Categoria());

        _repo.Setup(r => r.Update(existing));
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _service.UpdateAsync(1, updated, new List<int>(), 10, true);

        Assert.Equal("Título", existing.Titulo);
        Assert.Equal("contenido-limpio", existing.Contenido);
    }
}
