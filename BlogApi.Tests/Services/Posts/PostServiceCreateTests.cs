using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BlogApi.Tests.Services.Posts;

public class PostServiceCreateTests
{
    private readonly Mock<IPostRepository> _repo = new();
    private readonly Mock<ITagRepository> _tagRepo = new();
    private readonly Mock<ICategoriaRepository> _categoriaRepo = new();
    private readonly Mock<ISanitizerService> _sanitizer = new();
    private readonly Mock<INotificacionService> _notificaciones = new();

    private readonly PostService _service;

    public PostServiceCreateTests()
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
    // 1. Debe generar slug único
    // ------------------------------------------------------------
    [Fact]
    public async Task CreateAsync_ShouldGenerateUniqueSlug()
    {
        var post = new Post { Titulo = "Hola Mundo", CategoriaId = 1 };

        _categoriaRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Categoria());

        _tagRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<int>>())).ReturnsAsync(new List<Tag>());

        _repo
            .Setup(r =>
                r.Query().AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Post, bool>>>())
            )
            .ReturnsAsync(false);

        _sanitizer.Setup(s => s.SanitizePlainText("Hola Mundo")).Returns("Hola Mundo");
        _sanitizer.Setup(s => s.SanitizeMarkdown(It.IsAny<string>())).Returns("contenido");
        _sanitizer.Setup(s => s.ContainsDangerousPattern(It.IsAny<string>())).Returns(false);

        _repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(post, new List<int> { 1 }, usuarioId: 10);

        Assert.Equal("hola-mundo", result.Slug);
    }

    // ------------------------------------------------------------
    // 2. Debe generar slug incremental si existe
    // ------------------------------------------------------------
    [Fact]
    public async Task CreateAsync_ShouldGenerateIncrementalSlug_WhenSlugExists()
    {
        var post = new Post { Titulo = "Hola Mundo", CategoriaId = 1 };

        _categoriaRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Categoria());

        _tagRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<int>>())).ReturnsAsync(new List<Tag>());

        // Primera comprobación: slug existe
        _repo
            .SetupSequence(r =>
                r.Query().AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Post, bool>>>())
            )
            .ReturnsAsync(true) // "hola-mundo" existe
            .ReturnsAsync(false); // "hola-mundo-1" no existe

        _sanitizer.Setup(s => s.SanitizePlainText("Hola Mundo")).Returns("Hola Mundo");
        _sanitizer.Setup(s => s.SanitizeMarkdown(It.IsAny<string>())).Returns("contenido");
        _sanitizer.Setup(s => s.ContainsDangerousPattern(It.IsAny<string>())).Returns(false);

        _repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(post, new List<int> { 1 }, usuarioId: 10);

        Assert.Equal("hola-mundo-1", result.Slug);
    }

    // ------------------------------------------------------------
    // 3. Debe asignar tags correctamente
    // ------------------------------------------------------------
    [Fact]
    public async Task CreateAsync_ShouldAssignTags()
    {
        var post = new Post { Titulo = "Test", CategoriaId = 1 };

        var tags = new List<Tag>
        {
            new Tag { Id = 1, Nombre = "C#" },
            new Tag { Id = 2, Nombre = "Backend" },
        };

        _categoriaRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Categoria());

        _tagRepo.Setup(r => r.GetByIdsAsync(new List<int> { 1, 2 })).ReturnsAsync(tags);

        _repo
            .Setup(r =>
                r.Query().AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Post, bool>>>())
            )
            .ReturnsAsync(false);

        _sanitizer.Setup(s => s.SanitizePlainText(It.IsAny<string>())).Returns("Test");
        _sanitizer.Setup(s => s.SanitizeMarkdown(It.IsAny<string>())).Returns("contenido");
        _sanitizer.Setup(s => s.ContainsDangerousPattern(It.IsAny<string>())).Returns(false);

        _repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(post, new List<int> { 1, 2 }, usuarioId: 10);

        Assert.Equal(2, result.Tags.Count);
    }

    // ------------------------------------------------------------
    // 4. Debe sanitizar título y contenido
    // ------------------------------------------------------------
    [Fact]
    public async Task CreateAsync_ShouldSanitizeFields()
    {
        var post = new Post
        {
            Titulo = " <b>Hola</b> ",
            Contenido = "<script>alert(1)</script>",
            CategoriaId = 1,
        };

        _categoriaRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Categoria());
        _tagRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<int>>())).ReturnsAsync(new List<Tag>());

        _repo
            .Setup(r =>
                r.Query().AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Post, bool>>>())
            )
            .ReturnsAsync(false);

        _sanitizer.Setup(s => s.SanitizePlainText(" <b>Hola</b> ")).Returns("Hola");
        _sanitizer
            .Setup(s => s.SanitizeMarkdown("<script>alert(1)</script>"))
            .Returns("contenido-limpio");
        _sanitizer.Setup(s => s.ContainsDangerousPattern(It.IsAny<string>())).Returns(false);

        _repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(post, new List<int> { 1 }, usuarioId: 10);

        Assert.Equal("Hola", result.Titulo);
        Assert.Equal("contenido-limpio", result.Contenido);
    }

    // ------------------------------------------------------------
    // 5. Debe asignar usuario y fechas
    // ------------------------------------------------------------
    [Fact]
    public async Task CreateAsync_ShouldAssignUserAndDates()
    {
        var post = new Post { Titulo = "Test", CategoriaId = 1 };

        _categoriaRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Categoria());
        _tagRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<int>>())).ReturnsAsync(new List<Tag>());

        _repo
            .Setup(r =>
                r.Query().AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Post, bool>>>())
            )
            .ReturnsAsync(false);

        _sanitizer.Setup(s => s.SanitizePlainText(It.IsAny<string>())).Returns("Test");
        _sanitizer.Setup(s => s.SanitizeMarkdown(It.IsAny<string>())).Returns("contenido");
        _sanitizer.Setup(s => s.ContainsDangerousPattern(It.IsAny<string>())).Returns(false);

        _repo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(post, new List<int> { 1 }, usuarioId: 99);

        Assert.Equal(99, result.UsuarioId);
        Assert.NotEqual(default, result.FechaCreacion);
        Assert.NotEqual(default, result.FechaActualizacion);
    }
}
