using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Interfaz del servicio de plantillas
/// </summary>
public interface IEmailTemplateService
{
    /// <summary>
    /// Carga la plantilla seleccionada
    /// </summary>
    /// <param name="nombreArchivo"></param>
    /// <returns>string</returns>
    Task<string> CargarPlantillaAsync(string nombreArchivo);

    /// <summary>
    /// Reemplaza las variables con sus respectivos valores
    /// </summary>
    /// <param name="plantilla"></param>
    /// <param name="valores"></param>
    /// <returns>return</returns>
    string ReemplazarVariables(string plantilla, Dictionary<string, string> valores);
}
