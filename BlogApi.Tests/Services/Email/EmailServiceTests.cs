using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using Moq;

namespace BlogApi.Tests.Services.Email;

public class EmailServiceTests
{
    private readonly Mock<IEmailSettingsService> _settingsService = new();
    private readonly Mock<IEmailLogService> _logService = new();
    private readonly Mock<IEmailTemplateService> _templateService = new();

    private EmailService CreateService()
    {
        return new EmailService(
            _settingsService.Object,
            _logService.Object,
            _templateService.Object
        );
    }

    // ------------------------------------------------------------
    // Caso 1: Envío correcto
    // ------------------------------------------------------------
    [Fact]
    public async Task EnviarAsync_ShouldSendEmail_WhenSettingsAreValid()
    {
        _settingsService
            .Setup(s => s.ObtenerEntidadAsync())
            .ReturnsAsync(
                new EmailSettings
                {
                    Activo = true,
                    Host = "smtp.test.com",
                    Puerto = 587,
                    Usuario = "user@test.com",
                    Password = "123",
                    Remitente = "admin@test.com",
                    NombreRemitente = "Admin",
                    UsarSSL = true
                }
            );

        var service = CreateService();

        await service.EnviarAsync("dest@test.com", "Asunto", "Mensaje");

        _logService.Verify(
            l => l.RegistrarExitoAsync("dest@test.com", "Asunto", "SMTP"),
            Times.Once
        );
    }

    // ------------------------------------------------------------
    // Caso 2: Envío desactivado
    // ------------------------------------------------------------
    [Fact]
    public async Task EnviarAsync_ShouldThrow_WhenEmailSendingIsDisabled()
    {
        _settingsService
            .Setup(s => s.ObtenerEntidadAsync())
            .ReturnsAsync(new EmailSettings { Activo = false });

        var service = CreateService();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.EnviarAsync("dest@test.com", "Asunto", "Mensaje")
        );

        _logService.Verify(
            l => l.RegistrarExitoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never
        );
    }

    // ------------------------------------------------------------
    // Caso 3: Configuración incompleta
    // ------------------------------------------------------------
    [Fact]
    public async Task EnviarAsync_ShouldThrow_WhenSettingsAreIncomplete()
    {
        _settingsService
            .Setup(s => s.ObtenerEntidadAsync())
            .ReturnsAsync(
                new EmailSettings
                {
                    Activo = true,
                    Host = "",
                    Remitente = "",
                    Password = ""
                }
            );

        var service = CreateService();

        await Assert.ThrowsAsync<Exception>(() =>
            service.EnviarAsync("dest@test.com", "Asunto", "Mensaje")
        );

        _logService.Verify(
            l => l.RegistrarExitoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never
        );
    }
}
