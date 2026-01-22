using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.Services.Interfaces;

public interface IEmailTemplateService
{
    Task<string> CargarPlantillaAsync(string nombreArchivo);
    string ReemplazarVariables(string plantilla, Dictionary<string, string> valores);
}
