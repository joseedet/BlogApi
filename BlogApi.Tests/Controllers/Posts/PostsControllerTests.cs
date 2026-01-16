using System.Security.Claims;
using BlogApi.Controllers;
using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BlogApi.Tests;

public class PostsControllerTests
{
    private readonly Mock<IPostService> _service = new();
    private readonly PostsController _controller;
    //private readonly Mock<INotificacionesService> _notificaciones = new();
    private readonly Mock<INotificacionService> _notificaciones = new();

    public PostsControllerTests()
    {
        _controller = new PostsController(_service.Object, _notificaciones.Object);
    }

    // ------------------------------------------------------------
    // GET ALL
    // ------------------------------------------------------------
    [Fact]
    public async Task GetAll_ShouldReturnOk_WithPosts()
    {
        var posts = new List<Post>
        {
            new Post { Id = 1, Titulo = "Post 1" },
            new Post { Id = 2, Titulo = "Post 2" },
        };

        _service.Setup(s => s.GetAllAsync()).ReturnsAsync(posts);

        var result = await _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsAssignableFrom<IEnumerable<PostDto>>(ok.Value);

        Assert.Equal(2, data.Count());
    }
    [Fact]
    public async Task GetAll_ShouldReturnBadRequest_WhenServiceThrowsArgumentException()
    {
        _service.Setup(s => s.GetAllAsync()).ThrowsAsync(new ArgumentException("Error de prueba"));

        var result = await _controller.GetAll();

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(bad.Value);
    }

    // ------------------------------------------------------------
    // GET BY ID
    // ------------------------------------------------------------
    [Fact]
    public async Task GetById_ShouldReturnOk_WhenPostExists()
    {
        var post = new Post { Id = 1, Titulo = "Hola" };

        _service.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(post);

        var result = await _controller.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<PostDto>(ok.Value);

        Assert.Equal(1, data.Id);
        Assert.Equal("Hola", data.Titulo);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenPostDoesNotExist()
    {
        _service.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Post?)null);

        var result = await _controller.GetById(99);

        Assert.IsType<NotFoundResult>(result);
        _service.Verify(s => s.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetById_ShouldReturnBadRequest_WhenServiceThrowsArgumentException()
    {
        _service
            .Setup(s => s.GetByIdAsync(1))
            .ThrowsAsync(new ArgumentException("Error de prueba"));

        var result = await _controller.GetById(1);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var error = bad.Value!.ToString();
        Assert.Contains("Error de prueba", error);
    }

    // ------------------------------------------------------------
    // GET BY SLUG
    // ------------------------------------------------------------
    [Fact]
    public async Task GetBySlug_ShouldReturnOk_WhenPostExists()
    {
        var post = new Post { Id = 1, Slug = "hola-mundo" };
        _service.Setup(s => s.GetBySlugAsync("hola-mundo")).ReturnsAsync(post);
        var result = await _controller.GetBySlug("hola-mundo");
        var ok = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<PostDto>(ok.Value);
        Assert.Equal("hola-mundo", data.Slug);
    }

    [Fact]
    public async Task GetBySlug_ShouldReturnNotFound_WhenPostDoesNotExist()
    {
        _service.Setup(s => s.GetBySlugAsync("no-existe")).ReturnsAsync((Post?)null);
        var result = await _controller.GetBySlug("no-existe");
        Assert.IsType<NotFoundResult>(result);
    }

    // ------------------------------------------------------------
    // CREATE
    // ------------------------------------------------------------
    [Fact]
    public async Task Create_ShouldReturnCreated_WhenSuccessful()
    {
        var post = new Post { Id = 1, Titulo = "Nuevo" };

        _service
            .Setup(s =>
                s.CreateAsync(
                    It.IsAny<Post>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<int>() // usuarioId
                )
            )
            .ReturnsAsync(post);

        var dto = new CreatePostDto
        {
            Titulo = "Nuevo",
            Contenido = "Contenido",
            CategoriaId = 1,
            TagIds = new List<int> { 1 },
        };

        var result = await _controller.Create(dto);

        Assert.IsType<CreatedAtActionResult>(result);
    }

    // ------------------------------------------------------------
    // UPDATE
    // ------------------------------------------------------------
  
    [Fact]
    public async Task Update_ShouldReturnOkWithUpdatedPost_WhenSuccessful()
    {
        // Arrange: usuario simulado
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "123"), // usuarioId
                    new Claim(ClaimTypes.Role, "Administrador"), // para puedeEditarTodo = true
                },
                "TestAuth"
            )
        );

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user },
        };

        _service
            .Setup(s =>
                s.UpdateAsync(
                    1,
                    It.IsAny<Post>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<int>(), // usuarioId
                    It.IsAny<bool>() // puedeEditarTodo
                )
            )
            .ReturnsAsync(true);

        _service
            .Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync(new Post { Id = 1, Titulo = "Editado" });

        var dto = new CreatePostDto
        {
            Titulo = "Editado",
            Contenido = "Nuevo contenido",
            CategoriaId = 2,
            TagIds = new List<int>(),
        };

        // Act
        var result = await _controller.Update(1, dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<PostDto>(ok.Value);
        Assert.Equal(1, data.Id);
    }

    /// <summary>
    /// Actualiza un post y devuelve BadRequest si el servicio devuelve false
    /// </summary>
    /// <returns>BadRequestResult</returns>
    
    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenServiceReturnsFalse()
    {
        // Arrange: usuario simulado
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "123"),
                    new Claim(ClaimTypes.Role, "Autor"), // por ejemplo, no admin/editor
                },
                "TestAuth"
            )
        );

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user },
        };

        _service
            .Setup(s =>
                s.UpdateAsync(
                    1,
                    It.IsAny<Post>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<int>(), // usuarioId
                    It.IsAny<bool>() // puedeEditarTodo
                )
            )
            .ReturnsAsync(false);

        var dto = new CreatePostDto
        {
            Titulo = "Editado",
            Contenido = "Nuevo contenido",
            CategoriaId = 2,
            TagIds = new List<int>(),
        };

        // Act
        var result = await _controller.Update(1, dto);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    // ------------------------------------------------------------
    // DELETE
    // ------------------------------------------------------------
    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenSuccessful()
    {
        // Arrange: usuario simulado
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "123"), // usuarioId
                    new Claim(ClaimTypes.Role, "Administrador"), // puedeEliminarTodo = true
                },
                "TestAuth"
            )
        );

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user },
        };

        _service
            .Setup(s =>
                s.DeleteAsync(
                    1,
                    It.IsAny<int>(), // usuarioId
                    It.IsAny<bool>() // puedeEliminarTodo
                )
            )
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _service.Verify(s => s.DeleteAsync(1, It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
    }

    /// <summary>
    /// Elimina un post y devuelve NotFound si el post no existe
    /// </summary>
    /// <returns>NotFoundResult</returns>
    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenPostDoesNotExist()
    {
        // Arrange: usuario simulado
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "123"), // usuarioId
                    new Claim(ClaimTypes.Role, "Autor"), // no admin/editor → puedeEliminarTodo = false
                },
                "TestAuth"
            )
        );

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user },
        };

        _service
            .Setup(s =>
                s.DeleteAsync(
                    1,
                    It.IsAny<int>(), // usuarioId
                    It.IsAny<bool>() // puedeEliminarTodo
                )
            )
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);

        _service.Verify(s => s.DeleteAsync(1, It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
    }

    /// <summary>
    /// Elimina un post y devuelve BadRequest si el servicio lanza ArgumentException
    /// </summary>
    /// <returns>BadRequestObjectResult</returns>
    [Fact]
    public async Task Delete_ShouldReturnBadRequest_WhenServiceThrowsArgumentException()
    {
        // Arrange: usuario simulado
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "123"), // usuarioId
                    new Claim(ClaimTypes.Role, "Autor"), // no admin/editor → puedeEliminarTodo = false
                },
                "TestAuth"
            )
        );

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user },
        };

        _service
            .Setup(s =>
                s.DeleteAsync(
                    1,
                    It.IsAny<int>(), // usuarioId
                    It.IsAny<bool>() // puedeEliminarTodo
                )
            )
            .ThrowsAsync(new ArgumentException("Error de prueba"));

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(bad.Value);
    }

    /// <summary>
    /// Crea un post y devuelve BadRequest si el servicio devuelve null
    /// </summary>
    /// <returns>BadRequestResult</returns>
    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenServiceReturnsNull()
    {
        // Arrange: usuario simulado
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "123"), // usuarioId
                    new Claim(ClaimTypes.Role, "Autor"),
                },
                "TestAuth"
            )
        );

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user },
        };

        _service
            .Setup(s =>
                s.CreateAsync(
                    It.IsAny<Post>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<int>() // usuarioId
                )
            )
            .ReturnsAsync((Post?)null);

        var dto = new CreatePostDto
        {
            Titulo = "Nuevo",
            Contenido = "Contenido",
            CategoriaId = 1,
            TagIds = new List<int>(),
        };

        // Act
        var result = await _controller.Create(dto);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    /// <summary>
    /// Actualiza un post y devuelve BadRequest si el modelo es inválido
    /// </summary>
    /// <returns>BadRequestObjectResult</returns>
    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        var dto = new CreatePostDto
        {
            Titulo = "", // inválido cuando actives [Required]
            Contenido = "Contenido",
            CategoriaId = 1,
            TagIds = new List<int>(),
        };

        _controller.ModelState.AddModelError("Titulo", "Required");

        var result = await _controller.Create(dto);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Titulo", bad.Value.ToString());
    }

    /// <summary>
    /// Actualiza un post y devuelve BadRequest si el modelo es inválido
    /// </summary>
    /// <returns>BadRequestObjectResult</returns>
    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        var dto = new CreatePostDto
        {
            Titulo = "", // inválido cuando actives [Required]
            Contenido = "Contenido",
            CategoriaId = 1,
            TagIds = new List<int>(),
        };

        // Forzamos un error de validación
        _controller.ModelState.AddModelError("Titulo", "Required");

        var result = await _controller.Update(1, dto);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(bad.Value); // opcional, pero profesional
    }

    /// <summary>
    /// Actualiza un post y devuelve BadRequest si el servicio lanza ArgumentException
    /// </summary>
    /// <returns>BadRequestObjectResult</returns>
    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenServiceThrowsArgumentException()
    {
        // Arrange: usuario simulado
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "123"), // usuarioId
                    new Claim(ClaimTypes.Role, "Autor"), // no admin/editor → puedeEditarTodo = false
                },
                "TestAuth"
            )
        );

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user },
        };

        _service
            .Setup(s =>
                s.UpdateAsync(
                    1,
                    It.IsAny<Post>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<int>(), // usuarioId
                    It.IsAny<bool>() // puedeEditarTodo
                )
            )
            .ThrowsAsync(new ArgumentException("Error de prueba"));

        var dto = new CreatePostDto
        {
            Titulo = "Editado",
            Contenido = "Nuevo contenido",
            CategoriaId = 2,
            TagIds = new List<int>(),
        };

        // Act
        var result = await _controller.Update(1, dto);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(bad.Value);
    }

    /// <summary>
    /// Actualiza un post y devuelve NotFound si el post actualizado no puede ser cargado
    /// </summary>
    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenUpdatedPostCannotBeLoaded()
    {
        // Arrange: usuario simulado
        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "123"), // usuarioId
                    new Claim(ClaimTypes.Role, "Autor"), // no admin/editor → puedeEditarTodo = false
                },
                "TestAuth"
            )
        );

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user },
        };

        // UpdateAsync devuelve true → la actualización "funciona"
        _service
            .Setup(s =>
                s.UpdateAsync(
                    1,
                    It.IsAny<Post>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<int>(), // usuarioId
                    It.IsAny<bool>() // puedeEditarTodo
                )
            )
            .ReturnsAsync(true);

        // Pero GetByIdAsync devuelve null → no se puede cargar el post actualizado
        _service.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Post?)null);

        var dto = new CreatePostDto
        {
            Titulo = "Editado",
            Contenido = "Nuevo contenido",
            CategoriaId = 2,
            TagIds = new List<int>(),
        };

        // Act
        var result = await _controller.Update(1, dto);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
