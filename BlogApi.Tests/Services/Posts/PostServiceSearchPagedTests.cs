using BlogApi.Data;
using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BlogApi.Tests.Services.Posts;

public class PostServiceSearchPagedTests
{
    private readonly PostService _service;
    private readonly BlogDbContext _context;

    public PostServiceSearchPagedTests()
    {
        var options = new DbContextOptionsBuilder<BlogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new BlogDbContext(options);

        // Repositorio real
        IPostRepository repo = new PostRepository(_context);

        // Dependencias mock
        var tagRepo = new Mock<ITagRepository>();
        var categoriaRepo = new Mock<ICategoriaRepository>();
        var sanitizer = new Mock<ISanitizerService>();
        var notificaciones = new Mock<INotificacionService>();

        // Sanitización básica para evitar nulls
        sanitizer.Setup(s => s.SanitizePlainText(It.IsAny<string>())).Returns((string s) => s);

        sanitizer.Setup(s => s.SanitizeMarkdown(It.IsAny<string>())).Returns((string s) => s);

        sanitizer.Setup(s => s.ContainsDangerousPattern(It.IsAny<string>())).Returns(false);

        _service = new PostService(
            repo,
            tagRepo.Object,
            categoriaRepo.Object,
            sanitizer.Object,
            notificaciones.Object
        );

        // Seed de datos
        _context.Posts.AddRange(
            new Post
            {
                Id = 1,
                Titulo = "Hola mundo",
                Contenido = "Contenido de prueba",
                Categoria = new Categoria
                {
                    Id = 1,
                    Nombre = "Backend",
                    Slug = "backend",
                },
                Usuario = new Usuario { Id = 1, Nombre = "Jose" },
                Tags = new List<Tag>
                {
                    new Tag { Id = 1, Nombre = "C#" },
                },
            },
            new Post
            {
                Id = 2,
                Titulo = "Hola backend",
                Contenido = "Más contenido",
                Categoria = new Categoria
                {
                    Id = 1,
                    Nombre = "Backend",
                    Slug = "backend",
                },
                Usuario = new Usuario { Id = 2, Nombre = "Antonio" },
                Tags = new List<Tag>
                {
                    new Tag { Id = 2, Nombre = "API" },
                },
            },
            new Post
            {
                Id = 3,
                Titulo = "Frontend avanzado",
                Contenido = "React y más",
                Categoria = new Categoria
                {
                    Id = 2,
                    Nombre = "Frontend",
                    Slug = "frontend",
                },
                Usuario = new Usuario { Id = 3, Nombre = "Maria" },
                Tags = new List<Tag>
                {
                    new Tag { Id = 3, Nombre = "React" },
                },
            }
        );

        _context.SaveChanges();
    }

    // ------------------------------------------------------------
    // 1. Debe devolver resultados paginados
    // ------------------------------------------------------------
    [Fact]
    public async Task SearchPagedAsync_ShouldReturnPagedResults()
    {
        var result = await _service.SearchPagedAsync("hola", 1, 10);

        Assert.Equal(2, result.Total);
        Assert.Equal(2, result.Items.Count());
    }

    // ------------------------------------------------------------
    // 2. Debe devolver vacío si no hay coincidencias
    // ------------------------------------------------------------
    [Fact]
    public async Task SearchPagedAsync_ShouldReturnEmpty_WhenNoMatches()
    {
        var result = await _service.SearchPagedAsync("no-existe", 1, 10);

        Assert.Equal(0, result.Total);
        Assert.Empty(result.Items);
    }

    // ------------------------------------------------------------
    // 3. Debe paginar correctamente
    // ------------------------------------------------------------
    [Fact]
    public async Task SearchPagedAsync_ShouldPaginateCorrectly()
    {
        var result = await _service.SearchPagedAsync("hola", 1, 1);

        Assert.Equal(2, result.Total);
        Assert.Single(result.Items);
    }
}
