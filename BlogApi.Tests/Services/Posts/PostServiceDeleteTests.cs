using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Tests.Common;
using Moq;

namespace BlogApi.Tests.Services.Posts;

public class PostServiceDeleteTests : PostServiceTestBase
{
    /*private readonly Mock<IPostRepository> _repo = new();
    private readonly Mock<ITagRepository> _tagRepo = new();
    private readonly Mock<ICategoriaRepository> _categoriaRepo = new();
    private readonly Mock<ISanitizerService> _sanitizerService = new();
    private readonly Mock<INotificacionService> _notificationService = new();*/
    private readonly PostService _service;

    public PostServiceDeleteTests()
    {
        _service = CreateService();
    }

    // ------------------------------------------------------------
    // 1. Post no encontrado
    // ------------------------------------------------------------
    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenPostNotFound()
    {
        Repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Post?)null);

        var result = await _service.DeleteAsync(1, 1, puedeElimarTodo: false);

        Assert.False(result);
        Repo.Verify(r => r.Remove(It.IsAny<Post>()), Times.Never);
        Repo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    // ------------------------------------------------------------
    // 2. Post encontrado
    // ------------------------------------------------------------
    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenPostExists()
    {
        // Arrange: el usuario es el autor del post
        var post = new Post { Id = 1, UsuarioId = 123 };

        Repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
        Repo.Setup(r => r.Remove(post));
        Repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteAsync(
            id: 1,
            usuarioId: 123, // es el autor
            puedeElimarTodo: false // no admin/editor, pero es el autor → permitido
        );

        // Assert
        Assert.True(result);
        Repo.Verify(r => r.Remove(post), Times.Once);
        Repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    // ------------------------------------------------------------
    // 3. Debe llamar a Remove con el post correcto
    // ------------------------------------------------------------
    [Fact]
    public async Task DeleteAsync_ShouldCallRemoveWithCorrectPost()
    {
        var post = new Post { Id = 1 };

        Repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

        Repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _service.DeleteAsync(1, 1, false);

        Repo.Verify(r => r.Remove(post), Times.Once);
    }

    // ------------------------------------------------------------
    // 4. Debe llamar a SaveChangesAsync una vez
    // ------------------------------------------------------------
    [Fact]
    public async Task DeleteAsync_ShouldCallSaveChangesOnce()
    {
        var post = new Post { Id = 1 };

        Repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

        Repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _service.DeleteAsync(1, 1, false);

        Repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenUserHasNoPermissions()
    {
        //var service = CreateService();

        var post = new Post { Id = 1, UsuarioId = 999 }; // otro autor
        Repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

        var result = await _service.DeleteAsync(1, usuarioId: 123, puedeElimarTodo: false);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenUserIsAuthor()
    {
        var post = new Post { Id = 1, UsuarioId = 123 };

        Repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
        Repo.Setup(r => r.Remove(post));
        Repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(
            id: 1,
            usuarioId: 123, // es el autor
            puedeElimarTodo: false // no admin/editor
        );

        Assert.True(result);
        Repo.Verify(r => r.Remove(post), Times.Once);
        Repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenUserHasPermission()
    {
        var post = new Post { Id = 1, UsuarioId = 999 }; // otro autor

        Repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
        Repo.Setup(r => r.Remove(post));
        Repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(
            id: 1,
            usuarioId: 123,
            puedeElimarTodo: true // admin/editor
        );

        Assert.True(result);
        Repo.Verify(r => r.Remove(post), Times.Once);
        Repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenRepositoryThrows()
    {
        var post = new Post { Id = 1, UsuarioId = 123 };

        Repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
        Repo.Setup(r => r.Remove(post)).Throws(new Exception("DB error"));

        var result = await _service.DeleteAsync(id: 1, usuarioId: 123, puedeElimarTodo: true);

        Assert.False(result);
    }
}
