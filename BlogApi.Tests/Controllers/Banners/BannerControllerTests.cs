using BlogApi.Controllers;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BlogApi.Tests.Controllers;

public class BannerControllerTests
{
    private readonly Mock<IBannerService> _service = new();

    private BannerController CreateController()
    {
        return new BannerController(_service.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
        };
    }

    // ------------------------------------------------------------
    // GET /api/banner
    // ------------------------------------------------------------
    [Fact]
    public async Task ObtenerTodos_ShouldReturnOk()
    {
        _service.Setup(s => s.ObtenerTodosAsync()).ReturnsAsync(new List<BannerDto>());

        var controller = CreateController();

        var result = await controller.ObtenerTodos();

        Assert.IsType<OkObjectResult>(result.Result);
    }

    // ------------------------------------------------------------
    // GET /api/banner/activos
    // ------------------------------------------------------------
    [Fact]
    public async Task ObtenerActivos_ShouldReturnOk()
    {
        _service.Setup(s => s.ObtenerActivosAsync()).ReturnsAsync(new List<BannerDto>());

        var controller = CreateController();

        var result = await controller.ObtenerActivos();

        Assert.IsType<OkObjectResult>(result.Result);
    }

    // ------------------------------------------------------------
    // GET /api/banner/{id}
    // ------------------------------------------------------------
    [Fact]
    public async Task ObtenerPorId_ShouldReturnOk_WhenExists()
    {
        _service.Setup(s => s.ObtenerPorIdAsync(1)).ReturnsAsync(new BannerDto { Id = 1 });

        var controller = CreateController();

        var result = await controller.ObtenerPorId(1);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task ObtenerPorId_ShouldReturnNotFound_WhenNotExists()
    {
        _service.Setup(s => s.ObtenerPorIdAsync(1)).ReturnsAsync((BannerDto?)null);

        var controller = CreateController();

        var result = await controller.ObtenerPorId(1);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    // ------------------------------------------------------------
    // POST /api/banner
    // ------------------------------------------------------------
    [Fact]
    public async Task Crear_ShouldReturnCreated()
    {
        var dto = new BannerCreateDto { Titulo = "Test", ImagenFile = Mock.Of<IFormFile>() };

        _service.Setup(s => s.CrearAsync(dto)).ReturnsAsync(new BannerDto { Id = 1 });

        var controller = CreateController();

        var result = await controller.Crear(dto);

        Assert.IsType<CreatedAtActionResult>(result.Result);
    }

    // ------------------------------------------------------------
    // PUT /api/banner/{id}
    // ------------------------------------------------------------
    [Fact]
    public async Task Actualizar_ShouldReturnOk_WhenExists()
    {
        var dto = new BannerUpdateDto { Titulo = "Actualizado" };

        _service.Setup(s => s.ActualizarAsync(1, dto)).ReturnsAsync(new BannerDto { Id = 1 });

        var controller = CreateController();

        var result = await controller.Actualizar(1, dto);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task Actualizar_ShouldReturnNotFound_WhenNotExists()
    {
        var dto = new BannerUpdateDto { Titulo = "Actualizado" };

        _service.Setup(s => s.ActualizarAsync(1, dto)).ReturnsAsync((BannerDto?)null);

        var controller = CreateController();

        var result = await controller.Actualizar(1, dto);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    // ------------------------------------------------------------
    // DELETE /api/banner/{id}
    // ------------------------------------------------------------
    [Fact]
    public async Task Eliminar_ShouldReturnNoContent_WhenSuccessful()
    {
        _service.Setup(s => s.EliminarAsync(1)).ReturnsAsync(true);

        var controller = CreateController();

        var result = await controller.Eliminar(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Eliminar_ShouldReturnNotFound_WhenNotExists()
    {
        _service.Setup(s => s.EliminarAsync(1)).ReturnsAsync(false);

        var controller = CreateController();

        var result = await controller.Eliminar(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
