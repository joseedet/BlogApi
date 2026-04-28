using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlogApi.Filters;

/// <summary>
/// Filtro de acción personalizado para validar los modelos de entrada en las acciones del controlador. Si el modelo no es válido, devuelve una respuesta de error con los detalles de la validación. Este filtro se puede aplicar a nivel de controlador o a nivel de acción para garantizar que los datos recibidos cumplan con las reglas de validación definidas en los modelos.
/// </summary>
public class ValidationFilter : IActionFilter
{
    /// <summary>
    /// Método que se ejecuta antes de que se ejecute la acción del controlador. Verifica si el modelo de entrada es válido y, si no lo es, construye una respuesta de error con los detalles de la validación y la devuelve al cliente. Si el modelo es válido, permite que la acción del controlador se ejecute normalmente.
    /// </summary>
    /// <param name="context"></param>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            var response = new
            {
                Message = "Se han producido errores de validación.",
                Errors = errors
            };

            context.Result = new BadRequestObjectResult(response);
        }
    }
    /// <summary>
    /// Método que se ejecuta después de que se ejecute la acción del controlador. En este caso, no se necesita lógica adicional después de la ejecución de la acción, por lo que este método está vacío. Sin embargo, se puede utilizar para realizar tareas adicionales después de la ejecución de la acción, como registrar información o modificar la respuesta antes de enviarla al cliente.
    /// </summary>
    /// <param name="context"></param>

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No necesitamos lógica después de ejecutar la acción
    }
}
