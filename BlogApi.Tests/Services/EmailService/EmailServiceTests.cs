using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using Moq;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BlogApi.Tests.Services;

public class EmailServiceTests
{
    private readonly Mock<IEmailSettingsService> _settingsService = new();
    private readonly Mock<ISendGridClientFactory> _factory = new();
    private readonly Mock<SendGridClient> _sendGridMock;

    public EmailServiceTests()
    {
        _sendGridMock = new Mock<SendGridClient>("FAKE_API_KEY");
    }

    private EmailService CreateService()
    {
        _factory.Setup(f => f.Create(It.IsAny<string>())).Returns(_sendGridMock.Object);

        return new EmailService(_settingsService.Object, _factory.Object);
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
                    Password = "API_KEY",
                    Remitente = "admin@test.com",
                    NombreRemitente = "Admin",
                }
            );

        _sendGridMock
            .Setup(c => c.SendEmailAsync(It.IsAny<SendGridMessage>(), default))
            .ReturnsAsync(new Response(System.Net.HttpStatusCode.Accepted, null, null));

        var service = CreateService();

        await service.EnviarAsync("dest@test.com", "Asunto", "Mensaje");

        _sendGridMock.Verify(
            c => c.SendEmailAsync(It.IsAny<SendGridMessage>(), default),
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
                    Password = "",
                    Remitente = "",
                }
            );

        var service = CreateService();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.EnviarAsync("dest@test.com", "Asunto", "Mensaje")
        );
    }
}
