using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.Services;

public class EmailTemplateService
{
    public string CargarPlantilla(string nombreArchivo)
    {
        var ruta = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", nombreArchivo);
        return File.ReadAllText(ruta);
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
