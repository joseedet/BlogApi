using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.Services;

public class EmailTemplateService
{
    /// <summary>
    /// Carga una plantilla de correo electrónico desde un archivo
    /// </summary>
    /// <param name="nombreArchivo"></param>
    /// <returns></returns>
    /// </summary>
    public string CargarPlantilla(string nombreArchivo)
    {
        var ruta = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", nombreArchivo);
        return File.ReadAllText(ruta);
    }

    /// <summary>
    /// Reemplaza las variables en una plantilla con los valores proporcionados
    /// </summary>
    /// <param name="plantilla"></param>
    /// <param name="valores"></param>
    /// <returns>plantilla procesada</returns>
    /// </summary>
    public string ReemplazarVariables(string plantilla, Dictionary<string, string> valores)
    {
        foreach (var kv in valores)
        {
            plantilla = plantilla.Replace($"{{{{{kv.Key}}}}}", kv.Value);
        }
        return plantilla;
    }
}
