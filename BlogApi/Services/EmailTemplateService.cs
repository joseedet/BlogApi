using BlogApi.Services.Interfaces;

namespace BlogApi.Services;

/// <summary>
/// Servicio de plantillas de email
/// </summary>
public class EmailTemplateService : IEmailTemplateService
{
    private readonly IWebHostEnvironment _env;

    /// <summary>
    /// Constructor de EmailTemplateService
    /// </summary>
    /// <param name="env"></param>
    public EmailTemplateService(IWebHostEnvironment env)
    {
        _env = env;
    }

    /// <summary>
    /// Carga la plantilla
    /// </summary>
    /// <param name="nombreArchivo"></param>
    /// <returns>string</returns>
    /// <exception cref="FileNotFoundException"></exception>
    public async Task<string> CargarPlantillaAsync(string nombreArchivo)
    {
        var ruta = Path.Combine(_env.ContentRootPath, "EmailTemplates", nombreArchivo);

        if (!File.Exists(ruta))
            throw new FileNotFoundException($"No se encontró la plantilla: {ruta}");

        return await File.ReadAllTextAsync(ruta);
    }

    /// <summary>
    /// Reemplaza las variables
    /// </summary>
    /// <param name="plantilla"></param>
    /// <param name="valores"></param>
    /// <returns></returns>
    public string ReemplazarVariables(string plantilla, Dictionary<string, string> valores)
    {
        foreach (var kv in valores)
        {
            plantilla = plantilla.Replace($"{{{{{kv.Key}}}}}", kv.Value);
        }

        return plantilla;
    }

    /// <summary>
    /// Carga layout, plantilla especifica.Reemplazar variables específicas.Insertar contenido en layout.Reemplazar variables globales. Devolver HTML final
    /// </summary>
    /// <param name="templateName"></param>
    /// <param name="variables"></param>
    /// <returns>string</returns>
    public async Task<string> RenderTemplateAsync(
        string templateName,
        Dictionary<string, string> variables
    )
    {
        // 1. Cargar layout base
        var layout = await CargarPlantillaAsync("Base/layout.html");

        // 2. Cargar plantilla específica
        var content = await CargarPlantillaAsync(templateName);

        // 3. Reemplazar variables específicas en el contenido
        content = ReemplazarVariables(content, variables);

        // 4. Insertar contenido en el layout
        layout = layout.Replace("{{CONTENT}}", content);

        // 5. Variables globales
        var globalVars = new Dictionary<string, string>
        {
            { "APP_NAME", variables.GetValueOrDefault("APP_NAME", "Mi Aplicación") },
            { "SUBJECT", variables.GetValueOrDefault("SUBJECT", "") },
            { "YEAR", DateTime.UtcNow.Year.ToString() },
        };

        layout = ReemplazarVariables(layout, globalVars);

        return layout;
    }
    
}
