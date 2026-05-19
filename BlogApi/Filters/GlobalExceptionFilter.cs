using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlogApi.Filters;

/// <summary>
/// Filtro de excepción global para manejar las excepciones no controladas que ocurren en la aplicación. Este filtro captura cualquier excepción que no haya sido manejada en los controladores o servicios, registra el error y devuelve una respuesta de error adecuada al cliente. El filtro utiliza el tipo de excepción para determinar el código de estado HTTP y el mensaje de error que se devolverá, proporcionando una forma centralizada de manejar los errores en toda la aplicación. Esto mejora la robustez y la experiencia del usuario al garantizar que los errores se manejen de manera consistente y se proporcionen respuestas informativas en caso de fallos.  
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    /// <summary>
    /// Constructor del filtro de excepción global. Recibe una instancia del logger a través de la inyección de dependencias, lo que permite al filtro registrar los errores que ocurren en la aplicación. Este constructor es esencial para establecer la conexión entre el filtro y el sistema de registro, permitiendo que el filtro capture y registre las excepciones no controladas de manera efectiva. Al utilizar el logger, el filtro puede proporcionar información detallada sobre los errores, lo que facilita la depuración y el mantenimiento de la aplicación.
    /// </summary>
    /// <param name="logger"></param>
    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }
    /// <summary>
    /// Método que se ejecuta cuando se produce una excepción no controlada en la aplicación. Este método captura la excepción, registra el error utilizando el logger y construye una respuesta de error adecuada según el tipo de excepción. Dependiendo del tipo de excepción, se devuelve un código de estado HTTP específico (como 400 para errores de argumento, 404 para recursos no encontrados, 401 para acceso no autorizado o 500 para errores internos del servidor) junto con un mensaje de error detallado. Este enfoque centralizado para manejar las excepciones mejora la consistencia y la experiencia del usuario al proporcionar respuestas informativas en caso de errores.
    /// </summary>
    /// <param name="context"></param>
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        _logger.LogError(exception, "Excepción no controlada capturada por GlobalExceptionFilter.");

        ProblemDetails problem;

        switch (exception)
        {
            case ArgumentException argEx:
                problem = new ProblemDetails
                {
                    Title = "Error en los parámetros enviados.",
                    Detail = argEx.Message,
                    Status = (int)HttpStatusCode.BadRequest,
                    Type = "https://httpstatuses.com/400",
                };
                context.Result = new BadRequestObjectResult(problem);
                break;

            case KeyNotFoundException notFoundEx:
                problem = new ProblemDetails
                {
                    Title = "Recurso no encontrado.",
                    Detail = notFoundEx.Message,
                    Status = (int)HttpStatusCode.NotFound,
                    Type = "https://httpstatuses.com/404",
                };
                context.Result = new NotFoundObjectResult(problem);
                break;

            case UnauthorizedAccessException unauthEx:
                problem = new ProblemDetails
                {
                    Title = "Acceso no autorizado.",
                    Detail = unauthEx.Message,
                    Status = (int)HttpStatusCode.Unauthorized,
                    Type = "https://httpstatuses.com/401",
                };
                context.Result = new ObjectResult(problem)
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                };
                break;

            default:
                problem = new ProblemDetails
                {
                    Title = "Error interno del servidor.",
                    Detail = exception.Message,
                    Status = (int)HttpStatusCode.InternalServerError,
                    Type = "https://httpstatuses.com/500",
                };
                context.Result = new ObjectResult(problem)
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                };
                break;
        }

        context.ExceptionHandled = true;
    }
}
