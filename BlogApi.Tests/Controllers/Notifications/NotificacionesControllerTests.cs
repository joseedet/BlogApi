using System;
using System.Security.Claims;
using BlogApi.Controllers;
using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BlogApi.Tests.Controllers.Notifications;

public class NotificacionesControllerTests
{
    private readonly Mock<INotificacionService> _service = new();
    private readonly Mock<INotificacionRepository> _repo = new();

    private NotificacionesController CreateController(int usuarioId = 10)
    {
        var controller = new NotificacionesController(_service.Object, _repo.Object);

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
        var lista = new List<Notificacion>
        {
            new Notificacion
            {
                Id = 1,
                UsuarioDestinoId = 10,
                Mensaje = "Hola",
            },
        };

        _repo.Setup(r => r.ObtenerPorUsuarioAsync(10)).ReturnsAsync(lista);

        var controller = CreateController();

        var result = await controller.ObtenerNotificaciones();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsAssignableFrom<IEnumerable<NotificacionDto>>(ok.Value);

        Assert.Single(dto);
    }

    // ------------------------------------------------------------
    // PUT /api/notificaciones/{id}/leer
    // ------------------------------------------------------------
    [Fact]
    public async Task MarcarComoLeida_ShouldReturnOk_WhenSuccessful()
    {
        var notif = new Notificacion
        {
            Id = 1,
            UsuarioDestinoId = 10,
            Mensaje = "Hola",
        };

        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(notif);
        _repo.Setup(r => r.MarcarComoLeidaAsync(1)).Returns(Task.CompletedTask);

        var controller = CreateController();

        var result = await controller.MarcarComoLeida(1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task MarcarComoLeida_ShouldReturnNotFound_WhenNotExists()
    {
        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync((Notificacion?)null);

        var controller = CreateController();

        var result = await controller.MarcarComoLeida(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task MarcarComoLeida_ShouldReturnForbid_WhenUserNotOwner()
    {
        var notif = new Notificacion
        {
            Id = 1,
            UsuarioDestinoId = 999,
            Mensaje = "Hola",
        };

        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(notif);

        var controller = CreateController(usuarioId: 10);

        var result = await controller.MarcarComoLeida(1);

        Assert.IsType<ForbidResult>(result);
    }

    // ------------------------------------------------------------
    // PUT /api/notificaciones/leer-todas
    // ------------------------------------------------------------
    [Fact]
    public async Task MarcarTodasComoLeidas_ShouldReturnOk()
    {
        _service.Setup(s => s.MarcarTodasComoLeidasAsync(10)).Returns(Task.CompletedTask);

        var controller = CreateController();

        var result = await controller.MarcarTodasComoLeidas();

        Assert.IsType<OkObjectResult>(result);
    }

    // ------------------------------------------------------------
    // GET /api/notificaciones/no-leidas
    // ------------------------------------------------------------
    [Fact]
    public async Task ObtenerNoLeidasPaginadas_ShouldReturnOk()
    {
        var paginado = new PaginacionResultado<NotificacionDto>
        {
            Items = new List<NotificacionDto>(),
            TotalRegistros = 0,
            PaginaActual = 1,
            TotalPaginas = 1,
        };

        _repo.Setup(r => r.ObtenerNoLeidasPaginadasAsync(10, 1, 10)).ReturnsAsync(paginado);

        var controller = CreateController();

        var result = await controller.ObtenerNoLeidasPaginadas();

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task Eliminar_ShouldReturnOk_WhenSuccessful()
    {
        var notif = new Notificacion
        {
            Id = 1,
            UsuarioDestinoId = 10,
            Mensaje = "Hola",
        };

        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(notif);
        _repo.Setup(r => r.EliminarAsync(notif)).Returns(Task.CompletedTask);

        var controller = CreateController();

        var result = await controller.Eliminar(1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Eliminar_ShouldReturnNotFound_WhenNotExists()
    {
        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync((Notificacion?)null);

        var controller = CreateController();

        var result = await controller.Eliminar(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Eliminar_ShouldReturnForbid_WhenUserNotOwner()
    {
        var notif = new Notificacion
        {
            Id = 1,
            UsuarioDestinoId = 999,
            Mensaje = "Hola",
        };

        _repo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(notif);

        var controller = CreateController(usuarioId: 10);

        var result = await controller.Eliminar(1);

        Assert.IsType<ForbidResult>(result);
    }
}
