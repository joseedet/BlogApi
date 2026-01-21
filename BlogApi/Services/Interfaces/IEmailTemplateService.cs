using BlogApi.Services.Interfaces;

namespace BlogApi.Services;

public class EmailTemplateService : IEmailTemplateService
{
    private readonly IWebHostEnvironment _env;

    public EmailTemplateService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> CargarPlantillaAsync(string nombreArchivo)
    {
        var ruta = Path.Combine(_env.ContentRootPath, "EmailTemplates", nombreArchivo);

        if (!File.Exists(ruta))
            throw new FileNotFoundException($"No se encontró la plantilla: {ruta}");

        return await File.ReadAllTextAsync(ruta);
    }

    public string ReemplazarVariables(string plantilla, Dictionary<string, string> valores)
    {
        foreach (var kv in valores)
        {
            plantilla = plantilla.Replace($"{{{{{kv.Key}}}}}", kv.Value);
        }

        return plantilla;
    }
}
