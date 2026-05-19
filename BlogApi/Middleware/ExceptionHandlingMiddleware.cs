using System.Net;
using System.Text.Json;

namespace BlogApi.Middleware;

/// <summary>
/// Middleware para manejo global de excepciones no controladas. Este middleware captura cualquier excepción que no haya sido manejada en los controladores o servicios, registra el error y devuelve una respuesta de error adecuada al cliente. El middleware utiliza el tipo de excepción para determinar el código de estado HTTP y el mensaje de error que se devolverá, proporcionando una forma centralizada de manejar los errores en toda la aplicación. Esto mejora la robustez y la experiencia del usuario al garantizar que los errores se manejen de manera consistente y se proporcionen respuestas informativas en caso de fallos.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

/// <summary>
/// Constructor del middleware de manejo de excepciones. Recibe una instancia del siguiente middleware en la cadena y un logger a través de la inyección de dependencias, lo que permite al middleware registrar los errores que ocurren en la aplicación. Este constructor es esencial para establecer la conexión entre el middleware y el sistema de registro, permitiendo que el middleware capture y registre las excepciones no controladas de manera efectiva. Al utilizar el logger, el middleware puede proporcionar información detallada sobre los errores, lo que facilita la depuración y el mantenimiento de la aplicación.
/// </summary>
/// <param name="next"></param>
/// <param name="logger"></param>
    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
    }
    /// <summary>
    /// Método que se ejecuta para cada solicitud HTTP. Este método captura cualquier excepción no controlada que ocurra durante el procesamiento de la solicitud, registra el error utilizando el logger y construye una respuesta de error adecuada según el tipo de excepción. Dependiendo del tipo de excepción, se devuelve un código de estado HTTP específico (como 400 para errores de argumento, 404 para recursos no encontrados, 401 para acceso no autorizado o 500 para errores internos del servidor) junto con un mensaje de error detallado. Este enfoque centralizado para manejar las excepciones mejora la consistencia y la experiencia del usuario al proporcionar respuestas informativas en caso de errores. 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción no controlada capturada por el middleware global.");

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            ArgumentException => HttpStatusCode.BadRequest,
            KeyNotFoundException => HttpStatusCode.NotFound,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError,
        };

        var problem = new
        {
            type = $"https://httpstatuses.com/{(int)statusCode}",
            title = GetTitle(statusCode),
            status = (int)statusCode,
            detail = exception.Message,
        };

        var json = JsonSerializer.Serialize(problem);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(json);
    }

    private static string GetTitle(HttpStatusCode statusCode) =>
        statusCode switch
        {
            HttpStatusCode.BadRequest => "Error en los parámetros enviados.",
            HttpStatusCode.NotFound => "Recurso no encontrado.",
            HttpStatusCode.Unauthorized => "Acceso no autorizado.",
            _ => "Error interno del servidor.",
        };
}
