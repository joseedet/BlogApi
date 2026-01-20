using BlogApi.Controllers;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BlogApi.Tests.Controllers;

public class EmailSettingsControllerTests
{
    private readonly Mock<IEmailSettingsService> _settingsService = new();
    private readonly Mock<IEmailService> _emailService = new();

    private EmailSettingsController CreateController()
    {
        return new EmailSettingsController(_settingsService.Object, _emailService.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
        };
    }

    // ------------------------------------------------------------
    // GET /api/emailsettings
    // ------------------------------------------------------------
    [Fact]
    public async Task Obtener_ShouldReturnOk()
    {
        _settingsService.Setup(s => s.ObtenerAsync()).ReturnsAsync(new EmailSettingsDto());

        var controller = CreateController();

        var result = await controller.Obtener();

        Assert.IsType<OkObjectResult>(result.Result);
    }

    // ------------------------------------------------------------
    // PUT /api/emailsettings
    // ------------------------------------------------------------
    [Fact]
    public async Task Actualizar_ShouldReturnOk()
    {
        var dto = new EmailSettingsUpdateDto
        {
            Host = "smtp.test.com",
            Puerto = 587,
            Usuario = "user",
            Password = "pass",
            Remitente = "admin@test.com",
            NombreRemitente = "Admin",
            UsarSSL = true,
            Activo = true,
        };

        _settingsService.Setup(s => s.ActualizarAsync(dto)).ReturnsAsync(new EmailSettingsDto());

        var controller = CreateController();

        var result = await controller.Actualizar(dto);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    // ------------------------------------------------------------
    // POST /api/emailsettings/test (éxito)
    // ------------------------------------------------------------
    [Fact]
    public async Task EnviarEmailPrueba_ShouldReturnOk_WhenEmailSent()
    {
        var dto = new EmailTestRequest { Destinatario = "test@test.com" };

        _emailService
            .Setup(s => s.EnviarAsync(dto.Destinatario, It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var controller = CreateController();

        var result = await controller.EnviarEmailPrueba(dto);

        Assert.IsType<OkObjectResult>(result);
    }

    // ------------------------------------------------------------
    // POST /api/emailsettings/test (error)
    // ------------------------------------------------------------
    [Fact]
    public async Task EnviarEmailPrueba_ShouldReturnBadRequest_WhenEmailFails()
    {
        var dto = new EmailTestRequest { Destinatario = "test@test.com" };

        _emailService
            .Setup(s => s.EnviarAsync(dto.Destinatario, It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("SMTP error"));

        var controller = CreateController();

        var result = await controller.EnviarEmailPrueba(dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
