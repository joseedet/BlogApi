using BlogApi.Models;
using BlogApi.Services.Interfaces;
using BlogApi.Tests.Common;
using BlogApi.Utils;
using Moq;

namespace BlogApi.Tests.Services.Notificaciones;

public class NotificacionServiceTests : NotificacionServiceTestBase
{
    private readonly NotificacionService _service;

    public NotificacionServiceTests()
    {
        _service = CreateService();
    }

    // ------------------------------------------------------------
    // 1. Crear notificación
    // ------------------------------------------------------------
    [Fact]
    public async Task CrearAsync_ShouldInsertNotificationInDb()
    {
        var notificacion = new Notificacion
        {
            UsuarioId = 10,
            Mensaje = "Hola",
            Tipo = TipoNotificacion.NuevoPost,
        };

        await _service.CrearAsync(notificacion);

        Assert.Equal(1, Db.Notificaciones.Count());
    }

    // ------------------------------------------------------------
    // 2. Obtener notificaciones por usuario
    // ------------------------------------------------------------
    [Fact]
    public async Task GetByUsuarioAsync_ShouldReturnUserNotifications()
    {
        Db.Notificaciones.AddRange(
            new Notificacion { UsuarioId = 10, Mensaje = "A" },
            new Notificacion { UsuarioId = 10, Mensaje = "B" },
            new Notificacion { UsuarioId = 99, Mensaje = "C" }
        );
        await Db.SaveChangesAsync();

        var result = await _service.GetByUsuarioAsync(10);

        Assert.Equal(2, result.Count());
    }

    // ------------------------------------------------------------
    // 3. Marcar como leída — éxito
    // ------------------------------------------------------------
    [Fact]
    public async Task MarcarComoLeidaAsync_ShouldMarkNotificationAsRead()
    {
        var n = new Notificacion
        {
            Id = 1,
            UsuarioId = 10,
            Mensaje = "Test",
            Leida = false,
        };
        Db.Notificaciones.Add(n);
        await Db.SaveChangesAsync();

        var result = await _service.MarcarComoLeidaAsync(1, 10);

        Assert.True(result);
        Assert.True(n.Leida);
    }

    // ------------------------------------------------------------
    // 4. Marcar como leída — notificación no encontrada
    // ------------------------------------------------------------
    [Fact]
    public async Task MarcarComoLeidaAsync_ShouldReturnFalse_WhenNotFound()
    {
        var result = await _service.MarcarComoLeidaAsync(1, 10);

        Assert.False(result);
    }

    // ------------------------------------------------------------
    // 5. Marcar como leída — usuario sin permisos
    // ------------------------------------------------------------
    [Fact]
    public async Task MarcarComoLeidaAsync_ShouldReturnFalse_WhenUserHasNoPermission()
    {
        var notif = new Notificacion
        {
            Id = 1,
            UsuarioId = 999,
            Mensaje = "Test",
        };

        Repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(notif);

        var result = await _service.MarcarComoLeidaAsync(1, usuarioId: 10);

        Assert.False(result);
    }

    // ------------------------------------------------------------
    // 6. Eliminar — éxito
    // ------------------------------------------------------------
    [Fact]
    public async Task EliminarAsync_ShouldReturnTrue_WhenSuccessful()
    {
        var notif = new Notificacion
        {
            Id = 1,
            UsuarioId = 10,
            Mensaje = "Test",
        };

        Repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(notif);
        Repo.Setup(r => r.EliminarAsync(notif)).Returns(Task.CompletedTask);

        var result = await _service.EliminarAsync(1, usuarioId: 10);

        Assert.True(result);
        Repo.Verify(r => r.EliminarAsync(notif), Times.Once);

        // Verificar persistencia real
        await Db.SaveChangesAsync();
    }

    // ------------------------------------------------------------
    // 7. Eliminar — usuario sin permisos
    // ------------------------------------------------------------
    [Fact]
    public async Task EliminarAsync_ShouldReturnFalse_WhenUserHasNoPermission()
    {
        var notif = new Notificacion
        {
            Id = 1,
            UsuarioId = 999,
            Mensaje = "Test",
        };

        Repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(notif);

        var result = await _service.EliminarAsync(1, usuarioId: 10);

        Assert.False(result);
        Repo.Verify(r => r.EliminarAsync(It.IsAny<Notificacion>()), Times.Never);
    }

    // ------------------------------------------------------------
    // 8. Eliminar — notificación no encontrada
    // ------------------------------------------------------------
    [Fact]
    public async Task EliminarAsync_ShouldReturnFalse_WhenNotFound()
    {
        Repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Notificacion?)null);

        var result = await _service.EliminarAsync(1, usuarioId: 10);

        Assert.False(result);
    }

    // ------------------------------------------------------------
    // 9. Eliminar — excepción en repositorio
    // ------------------------------------------------------------
    [Fact]
    public async Task EliminarAsync_ShouldReturnFalse_WhenRepositoryThrows()
    {
        var notif = new Notificacion
        {
            Id = 1,
            UsuarioId = 10,
            Mensaje = "Test",
        };

        Repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(notif);
        Repo.Setup(r => r.EliminarAsync(notif)).Throws(new Exception("DB error"));

        var result = await _service.EliminarAsync(1, usuarioId: 10);

        Assert.False(result);
    }
}
