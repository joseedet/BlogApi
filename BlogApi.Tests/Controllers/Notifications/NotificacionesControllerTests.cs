using System.Security.Claims;
using BlogApi.Controllers;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BlogApi.Tests.Controllers;

public class NotificacionesControllerTests
{
    private readonly Mock<INotificacionesService> _service = new();

    private NotificacionesController CreateController(int usuarioId = 10)
    {
        var controller = new NotificacionesController(_service.Object);

        var user = new ClaimsPrincipal(
            new ClaimsIdentity(new[] { new Claim("id", usuarioId.ToString()) }, "mock")
        );

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user },
        };

        return controller;
    }

    // ------------------------------------------------------------
    // GET /api/notificaciones
    // ------------------------------------------------------------
    [Fact]
    public async Task ObtenerNotificaciones_ShouldReturnOk()
    {
        _service.Setup(s => s.ObtenerPorUsuarioAsync(10)).ReturnsAsync(new List<NotificacionDto>());

        var controller = CreateController();

        var result = await controller.ObtenerNotificaciones();

        Assert.IsType<OkObjectResult>(result.Result);
    }

    // ------------------------------------------------------------
    // GET /api/notificaciones/no-leidas
    // ------------------------------------------------------------
    [Fact]
    public async Task ObtenerNoLeidas_ShouldReturnOk()
    {
        _service.Setup(s => s.ObtenerNoLeidasAsync(10)).ReturnsAsync(new List<NotificacionDto>());

        var controller = CreateController();

        var result = await controller.ObtenerNoLeidas();

        Assert.IsType<OkObjectResult>(result.Result);
    }

    // ------------------------------------------------------------
    // GET /api/notificaciones/paginadas
    // ------------------------------------------------------------
    [Fact]
    public async Task ObtenerPaginadas_ShouldReturnOk()
    {
        var paginado = new PaginacionResultado<NotificacionDto>
        {
            Items = new List<NotificacionDto>(),
            TotalRegistros = 10,
            PaginaActual = 1,
            TotalPaginas = 1,
        };

        _service.Setup(s => s.GetPaginadasAsync(10, 1, 10)).ReturnsAsync(paginado);

        var controller = CreateController();

        var result = await controller.ObtenerPaginadas(1, 10);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    // ------------------------------------------------------------
    // PUT /api/notificaciones/{id}/leer
    // ------------------------------------------------------------
    [Fact]
    public async Task MarcarComoLeida_ShouldReturnNoContent_WhenSuccessful()
    {
        _service.Setup(s => s.MarcarComoLeidaAsync(1, 10)).ReturnsAsync(true);

        var controller = CreateController();

        var result = await controller.MarcarComoLeida(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task MarcarComoLeida_ShouldReturnNotFound_WhenNotExists()
    {
        _service.Setup(s => s.MarcarComoLeidaAsync(1, 10)).ReturnsAsync(false);

        var controller = CreateController();

        var result = await controller.MarcarComoLeida(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // ------------------------------------------------------------
    // PUT /api/notificaciones/leer-todas
    // ------------------------------------------------------------
    [Fact]
    public async Task MarcarTodasComoLeidas_ShouldReturnNoContent()
    {
        _service.Setup(s => s.MarcarTodasComoLeidasAsync(10)).Returns(Task.CompletedTask);

        var controller = CreateController();

        var result = await controller.MarcarTodasComoLeidas();

        Assert.IsType<NoContentResult>(result);
    }

    // ------------------------------------------------------------
    // DELETE /api/notificaciones/{id}
    // ------------------------------------------------------------
    [Fact]
    public async Task Eliminar_ShouldReturnNoContent_WhenSuccessful()
    {
        _service.Setup(s => s.EliminarAsync(1, 10)).ReturnsAsync(true);

        var controller = CreateController();

        var result = await controller.Eliminar(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Eliminar_ShouldReturnNotFound_WhenNotExists()
    {
        _service.Setup(s => s.EliminarAsync(1, 10)).ReturnsAsync(false);

        var controller = CreateController();

        var result = await controller.Eliminar(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
