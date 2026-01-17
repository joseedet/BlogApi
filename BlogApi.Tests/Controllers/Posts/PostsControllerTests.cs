using BlogApi.Controllers;
using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Tests.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BlogApi.Tests;

public class PostsControllerTests : ControllerTestBase
{
    // ------------------------------------------------------------
    // GET ALL
    // ------------------------------------------------------------
    [Fact]
    public async Task GetAll_ShouldReturnOk_WithPosts()
    {
        var controller = CreateController<PostsController>();

        var posts = new List<Post>
        {
            new Post { Id = 1, Titulo = "Post 1" },
            new Post { Id = 2, Titulo = "Post 2" },
        };

        PostService.Setup(s => s.GetAllAsync()).ReturnsAsync(posts);

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsAssignableFrom<IEnumerable<PostDto>>(ok.Value);

        Assert.Equal(2, data.Count());
    }

    [Fact]
    public async Task GetAll_ShouldReturnBadRequest_WhenServiceThrowsArgumentException()
    {
        var controller = CreateController<PostsController>();

        PostService
            .Setup(s => s.GetAllAsync())
            .ThrowsAsync(new ArgumentException("Error de prueba"));

        var result = await controller.GetAll();

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(bad.Value);
    }

    // ------------------------------------------------------------
    // GET BY ID
    // ------------------------------------------------------------
    [Fact]
    public async Task GetById_ShouldReturnOk_WhenPostExists()
    {
        var controller = CreateController<PostsController>();

        var post = new Post { Id = 1, Titulo = "Hola" };

        PostService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(post);

        var result = await controller.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<PostDto>(ok.Value);

        Assert.Equal(1, data.Id);
        Assert.Equal("Hola", data.Titulo);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenPostDoesNotExist()
    {
        var controller = CreateController<PostsController>();

        PostService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Post?)null);

        var result = await controller.GetById(99);

        Assert.IsType<NotFoundResult>(result);
        PostService.Verify(s => s.GetByIdAsync(99), Times.Once);
    }

    [Fact]
    public async Task GetById_ShouldReturnBadRequest_WhenServiceThrowsArgumentException()
    {
        var controller = CreateController<PostsController>();

        PostService
            .Setup(s => s.GetByIdAsync(1))
            .ThrowsAsync(new ArgumentException("Error de prueba"));

        var result = await controller.GetById(1);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Error de prueba", bad.Value!.ToString());
    }

    // ------------------------------------------------------------
    // GET BY SLUG
    // ------------------------------------------------------------
    [Fact]
    public async Task GetBySlug_ShouldReturnOk_WhenPostExists()
    {
        var controller = CreateController<PostsController>();

        var post = new Post { Id = 1, Slug = "hola-mundo" };
        PostService.Setup(s => s.GetBySlugAsync("hola-mundo")).ReturnsAsync(post);

        var result = await controller.GetBySlug("hola-mundo");

        var ok = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<PostDto>(ok.Value);

        Assert.Equal("hola-mundo", data.Slug);
    }

    [Fact]
    public async Task GetBySlug_ShouldReturnNotFound_WhenPostDoesNotExist()
    {
        var controller = CreateController<PostsController>();

        PostService.Setup(s => s.GetBySlugAsync("no-existe")).ReturnsAsync((Post?)null);

        var result = await controller.GetBySlug("no-existe");

        Assert.IsType<NotFoundResult>(result);
    }

    // ------------------------------------------------------------
    // CREATE
    // ------------------------------------------------------------
    [Fact]
    public async Task Create_ShouldReturnCreated_WhenSuccessful()
    {
        var controller = CreateController<PostsController>();
        SetUser(controller, FakeUser(role: "Autor"));

        var post = new Post { Id = 1, Titulo = "Nuevo" };

        PostService
            .Setup(s => s.CreateAsync(It.IsAny<Post>(), It.IsAny<List<int>>(), It.IsAny<int>()))
            .ReturnsAsync(post);

        var dto = new CreatePostDto
        {
            Titulo = "Nuevo",
            Contenido = "Contenido",
            CategoriaId = 1,
            TagIds = new List<int> { 1 },
        };

        var result = await controller.Create(dto);

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenServiceReturnsNull()
    {
        var controller = CreateController<PostsController>();
        SetUser(controller, FakeUser());

        PostService
            .Setup(s => s.CreateAsync(It.IsAny<Post>(), It.IsAny<List<int>>(), It.IsAny<int>()))
            .ReturnsAsync((Post?)null);

        var dto = new CreatePostDto
        {
            Titulo = "Nuevo",
            Contenido = "Contenido",
            CategoriaId = 1,
            TagIds = new List<int>(),
        };

        var result = await controller.Create(dto);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        var controller = CreateController<PostsController>();

        controller.ModelState.AddModelError("Titulo", "Required");

        var dto = new CreatePostDto
        {
            Titulo = "",
            Contenido = "Contenido",
            CategoriaId = 1,
            TagIds = new List<int>(),
        };

        var result = await controller.Create(dto);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Titulo", bad.Value.ToString());
    }

    // ------------------------------------------------------------
    // UPDATE
    // ------------------------------------------------------------
    [Fact]
    public async Task Update_ShouldReturnOkWithUpdatedPost_WhenSuccessful()
    {
        var controller = CreateController<PostsController>();
        SetUser(controller, FakeUser(role: "Administrador"));

        PostService
            .Setup(s =>
                s.UpdateAsync(
                    1,
                    It.IsAny<Post>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>()
                )
            )
            .ReturnsAsync(true);

        PostService
            .Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync(new Post { Id = 1, Titulo = "Editado" });

        var dto = new CreatePostDto
        {
            Titulo = "Editado",
            Contenido = "Nuevo contenido",
            CategoriaId = 2,
            TagIds = new List<int>(),
        };

        var result = await controller.Update(1, dto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<PostDto>(ok.Value);

        Assert.Equal(1, data.Id);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        var controller = CreateController<PostsController>();

        controller.ModelState.AddModelError("Titulo", "Required");

        var dto = new CreatePostDto
        {
            Titulo = "",
            Contenido = "Contenido",
            CategoriaId = 1,
            TagIds = new List<int>(),
        };

        var result = await controller.Update(1, dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenServiceReturnsFalse()
    {
        var controller = CreateController<PostsController>();
        SetUser(controller, FakeUser(role: "Autor"));

        PostService
            .Setup(s =>
                s.UpdateAsync(
                    1,
                    It.IsAny<Post>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>()
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

        var result = await controller.Update(1, dto);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenServiceThrowsArgumentException()
    {
        var controller = CreateController<PostsController>();
        SetUser(controller, FakeUser(role: "Autor"));

        PostService
            .Setup(s =>
                s.UpdateAsync(
                    1,
                    It.IsAny<Post>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>()
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

        var result = await controller.Update(1, dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenUpdatedPostCannotBeLoaded()
    {
        var controller = CreateController<PostsController>();
        SetUser(controller, FakeUser(role: "Autor"));

        PostService
            .Setup(s =>
                s.UpdateAsync(
                    1,
                    It.IsAny<Post>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>()
                )
            )
            .ReturnsAsync(true);

        PostService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Post?)null);

        var dto = new CreatePostDto
        {
            Titulo = "Editado",
            Contenido = "Nuevo contenido",
            CategoriaId = 2,
            TagIds = new List<int>(),
        };

        var result = await controller.Update(1, dto);

        Assert.IsType<NotFoundResult>(result);
    }

    // ------------------------------------------------------------
    // DELETE
    // ------------------------------------------------------------
    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenSuccessful()
    {
        var controller = CreateController<PostsController>();
        SetUser(controller, FakeUser(role: "Administrador"));

        PostService
            .Setup(s => s.DeleteAsync(1, It.IsAny<int>(), It.IsAny<bool>()))
            .ReturnsAsync(true);

        var result = await controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
        PostService.Verify(s => s.DeleteAsync(1, It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenPostDoesNotExist()
    {
        var controller = CreateController<PostsController>();
        SetUser(controller, FakeUser(role: "Autor"));

        PostService
            .Setup(s => s.DeleteAsync(1, It.IsAny<int>(), It.IsAny<bool>()))
            .ReturnsAsync(false);

        var result = await controller.Delete(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnBadRequest_WhenServiceThrowsArgumentException()
    {
        var controller = CreateController<PostsController>();
        SetUser(controller, FakeUser(role: "Autor"));

        PostService
            .Setup(s => s.DeleteAsync(1, It.IsAny<int>(), It.IsAny<bool>()))
            .ThrowsAsync(new ArgumentException("Error de prueba"));

        var result = await controller.Delete(1);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
