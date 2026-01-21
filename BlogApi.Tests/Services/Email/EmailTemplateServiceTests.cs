using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.Tests.Services.Email;

public class EmailTemplateServiceTests
{
    [Fact]
    public async Task CargarPlantillaAsync_ShouldLoadFile()
    {
        var env = new Mock<IWebHostEnvironment>();
        env.Setup(e => e.ContentRootPath).Returns(Directory.GetCurrentDirectory());
        var service = new EmailTemplateService(env.Object);
        var ruta = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "Test.html");
        Directory.CreateDirectory(Path.GetDirectoryName(ruta)!);
        await File.WriteAllTextAsync(ruta, "Hola {{NOMBRE}}");
        var contenido = await service.CargarPlantillaAsync("Test.html");
        Assert.Equal("Hola {{NOMBRE}}", contenido);
    }

    [Fact]
    public void ReemplazarVariables_ShouldReplaceCorrectly()
    {
        var service = new EmailTemplateService(Mock.Of<IWebHostEnvironment>());
        var plantilla = "Hola {{NOMBRE}}";
        var valores = new Dictionary<string, string> { { "NOMBRE", "Jose" } };
        var resultado = service.ReemplazarVariables(plantilla, valores);
        Assert.Equal("Hola Jose", resultado);
    }
}
